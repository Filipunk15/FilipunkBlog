using System.Net;
using System.Net.Mail;
using System.Net.Http.Json;
using FilipunkBlog.Application.Contracts;
using FilipunkBlog.Application.Models;
using FilipunkBlog.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FilipunkBlog.Infrastructure.Services;

public class NotificationService(
    IOptions<SmtpOptions> smtpOptions,
    IOptions<DiscordOptions> discordOptions,
    HttpClient httpClient,
    ILogger<NotificationService> logger) : INotificationService
{
    private readonly SmtpOptions _smtp = smtpOptions.Value;
    private readonly DiscordOptions _discord = discordOptions.Value;

    public async Task NotifyContactAsync(ContactMessage message)
    {
        var subject = $"Nová zpráva z webu od {message.Name}";
        var body =
            $"Jméno: {message.Name}\n" +
            $"E-mail: {message.Email}\n\n" +
            message.Message;

        await SendEmailAsync(subject, body, replyTo: message.Email);
        await SendDiscordAsync(_discord.ContactWebhook,
            $"📨 **Nová zpráva z kontaktního formuláře**\n" +
            $"**Od:** {message.Name} (`{message.Email}`)\n" +
            $"> {Truncate(message.Message, 500)}");
    }

    public async Task NotifyNewCommentAsync(string author, string postTitle, string excerpt)
    {
        await SendDiscordAsync(_discord.CommentWebhook,
            $"💬 **Nový komentář ke schválení** — *{postTitle}*\n" +
            $"**Od:** {author}\n" +
            $"> {Truncate(excerpt, 500)}");
    }

    private async Task SendEmailAsync(string subject, string body, string? replyTo)
    {
        if (!_smtp.IsConfigured)
        {
            logger.LogWarning("SMTP není nakonfigurováno – e-mail se neposílá.");
            return;
        }

        try
        {
            using var client = new SmtpClient(_smtp.Host, _smtp.Port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_smtp.Username, _smtp.Password),
            };

            using var mail = new MailMessage
            {
                From = new MailAddress(_smtp.From!, _smtp.FromName ?? _smtp.From),
                Subject = subject,
                Body = body,
            };
            mail.To.Add(_smtp.From!);
            if (!string.IsNullOrWhiteSpace(replyTo) && MailAddress.TryCreate(replyTo, out var addr))
                mail.ReplyToList.Add(addr);

            await client.SendMailAsync(mail);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Odeslání notifikačního e-mailu selhalo.");
        }
    }

    private async Task SendDiscordAsync(string? webhookUrl, string content)
    {
        if (string.IsNullOrWhiteSpace(webhookUrl))
        {
            logger.LogWarning("Discord webhook není nakonfigurován – zpráva se neposílá.");
            return;
        }

        try
        {
            var resp = await httpClient.PostAsJsonAsync(webhookUrl, new { content });
            if (!resp.IsSuccessStatusCode)
                logger.LogError("Discord webhook vrátil {Status}.", resp.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Odeslání Discord notifikace selhalo.");
        }
    }

    private static string Truncate(string value, int max)
        => value.Length <= max ? value : value[..max] + "…";
}
