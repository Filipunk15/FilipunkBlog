namespace FilipunkBlog.Domain.Entities;

/// <summary>Překlad článku do jiného jazyka. Základní pole <see cref="BlogPost"/> jsou česky.</summary>
public class BlogPostTranslation : EntityBase
{
    public Guid BlogPostId { get; set; }
    public BlogPost BlogPost { get; set; } = null!;

    /// <summary>Dvoupísmenný kód jazyka, např. "en".</summary>
    public string Culture { get; set; } = "en";

    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Introduction { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
