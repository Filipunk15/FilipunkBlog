namespace FilipunkBlog.Application.Models;

public class BlogPostViewModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    /// <summary>Slug v jednotlivých jazycích – pro hreflang na detailu.</summary>
    public string SlugCs { get; set; } = string.Empty;
    public string? SlugEn { get; set; }
    public string Introduction { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Image { get; set; }
    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategorySlug { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = [];
    public string ReadingTime => $"{Math.Max(1, Content.Split(' ').Length / 200)} min";
    public string PublishedAtDisplay => PublishedAt?.ToString("d. MMMM yyyy") ?? string.Empty;
    public Guid CategoryId { get; set; }

    public List<Guid> TagIds { get; set; } = [];

    public int Views { get; set; }

    /// <summary>Překlady obsahu (jen pro admin editaci). Na veřejném webu je Title/Content už ve správném jazyce.</summary>
    public List<ContentTranslationViewModel> Translations { get; set; } = [];
}