using FilipunkBlog.Application.Models;

namespace FilipunkBlog.Application.Contracts;

public interface INotificationService
{
    /// <summary>Zpráva z kontaktního formuláře → e-mail + Discord. Nikdy nevyhazuje výjimku.</summary>
    Task NotifyContactAsync(ContactMessage message);

    /// <summary>Nový komentář čeká na schválení → Discord. Nikdy nevyhazuje výjimku.</summary>
    Task NotifyNewCommentAsync(string author, string postTitle, string excerpt);
}
