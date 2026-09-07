namespace FilipunkBlog.Domain.Entities;

/// <summary>Certifikát / kurz. Zobrazuje se jen v životopise (<c>/cv.pdf</c>).</summary>
public class Certification : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public int? Year { get; set; }
    public string? Url { get; set; }
    public int SortOrder { get; set; }
}
