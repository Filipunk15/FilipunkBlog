namespace FilipunkBlog.Application.Models;

/// <summary>
/// Kompletní podklad pro vygenerování životopisu. Čisté DTO – veškeré řetězce (včetně
/// nadpisů sekcí a popisků období) jsou už ve správném jazyce, renderer nemá žádnou logiku.
/// </summary>
public class CvDocumentModel
{
    public string FullName { get; set; } = string.Empty;
    public string Headline { get; set; } = string.Empty;
    public string? Summary { get; set; }

    public List<CvContactLine> Contact { get; set; } = [];
    public List<CvEntry> Experience { get; set; } = [];
    public List<CvEntry> Education { get; set; } = [];
    public List<CvCertification> Certifications { get; set; } = [];
    public List<CvProject> Projects { get; set; } = [];

    public CvLabels Labels { get; set; } = new();
}

public record CvContactLine(string Label, string Value, string? Url);

public class CvEntry
{
    public string Title { get; set; } = string.Empty;
    public string Organization { get; set; } = string.Empty;
    public string PeriodLabel { get; set; } = string.Empty;
    public List<string> Bullets { get; set; } = [];
}

public record CvCertification(string Name, string Issuer, int? Year);

public record CvProject(string Title, string Description);

public class CvLabels
{
    public string Contact { get; set; } = "KONTAKT";
    public string Experience { get; set; } = "ZKUŠENOSTI";
    public string Education { get; set; } = "VZDĚLÁNÍ";
    public string Certificates { get; set; } = "CERTIFIKÁTY";
    public string Projects { get; set; } = "PROJEKTY";
}
