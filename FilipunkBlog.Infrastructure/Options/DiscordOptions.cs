namespace FilipunkBlog.Infrastructure.Options;

public class DiscordOptions
{
    public const string SectionName = "Discord";

    /// <summary>Obecný kanál – notifikace o nových komentářích.</summary>
    public string? WebhookUrl { get; set; }

    /// <summary>Kanál pro zprávy z kontaktního formuláře. Když není vyplněn, použije se <see cref="WebhookUrl"/>.</summary>
    public string? ContactWebhookUrl { get; set; }

    /// <summary>Webhook pro komentáře (nebo null).</summary>
    public string? CommentWebhook => Normalize(WebhookUrl);

    /// <summary>Webhook pro kontaktní formulář – vlastní kanál, jinak spadne zpět na obecný.</summary>
    public string? ContactWebhook => Normalize(ContactWebhookUrl) ?? Normalize(WebhookUrl);

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
