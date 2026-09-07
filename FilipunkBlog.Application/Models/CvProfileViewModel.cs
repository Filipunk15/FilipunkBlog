namespace FilipunkBlog.Application.Models;

public class CvProfileViewModel
{
    public string FullName { get; set; } = string.Empty;
    public string Headline { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? GitHubUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? Summary { get; set; }
}
