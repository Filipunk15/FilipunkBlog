namespace FilipunkBlog.Application.Models;

public class CertificationViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public int? Year { get; set; }
    public string? Url { get; set; }
    public int SortOrder { get; set; }
}
