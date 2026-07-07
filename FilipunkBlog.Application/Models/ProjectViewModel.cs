namespace FilipunkBlog.Application.Models;

public class ProjectViewModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
    public int StartYear { get; set; }
    public int? EndYear { get; set; }
    public string? MainImage { get; set; }
    public List<string> Images { get; set; } = [];
    public bool IsPublished { get; set; }
    public string YearDisplay => EndYear == null
    ? $"{StartYear}–nyní"
    : StartYear == EndYear
        ? $"{StartYear}"
        : $"{StartYear}–{EndYear}";
    public string PublishedAtDisplay => PublishedAt?.ToString("d. MMMM yyyy") ?? string.Empty;
    public DateTime? PublishedAt { get; set; }
    public List<string> Tags { get; set; } = [];
    public List<Guid> TagIds { get; set; } = [];
}