namespace FilipunkBlog.Domain.Entities;

/// <summary>Singleton – hlavička a kontaktní údaje životopisu. Zdroj pro <c>/cv.pdf</c>.</summary>
public class CvProfile : EntityBase
{
    public string FullName { get; set; } = string.Empty;
    public string Headline { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? GitHubUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? WebsiteUrl { get; set; }

    /// <summary>Nepovinný profesní medailonek (odstavec).</summary>
    public string? Summary { get; set; }
}
