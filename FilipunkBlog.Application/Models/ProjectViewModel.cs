namespace FilipunkBlog.Application.Models;

public class ProjectViewModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    /// <summary>Slug v jednotlivých jazycích – pro hreflang na detailu.</summary>
    public string SlugCs { get; set; } = string.Empty;
    public string? SlugEn { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
    public int StartYear { get; set; }
    public int? EndYear { get; set; }
    public string? GitHubUrl { get; set; }
    public string? LiveDemoUrl { get; set; }
    public bool IsFeatured { get; set; }
    public string? MainImage { get; set; }
    public List<string> Images { get; set; } = [];
    public bool IsPublished { get; set; }
    public string YearDisplay => EndYear == null
    ? $"{StartYear}–{(System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "en" ? "now" : "nyní")}"
    : StartYear == EndYear
        ? $"{StartYear}"
        : $"{StartYear}–{EndYear}";
    public string PublishedAtDisplay => PublishedAt?.ToString("d. MMMM yyyy") ?? string.Empty;
    public DateTime? PublishedAt { get; set; }
    public List<string> Tags { get; set; } = [];
    public List<Guid> TagIds { get; set; } = [];

    /// <summary>Překlady obsahu (jen pro admin editaci).</summary>
    public List<ContentTranslationViewModel> Translations { get; set; } = [];
}