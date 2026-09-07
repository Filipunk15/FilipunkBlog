namespace FilipunkBlog.Domain.Entities;

/// <summary>Překlad projektu do jiného jazyka. Základní pole <see cref="Project"/> jsou česky.</summary>
public class ProjectTranslation : EntityBase
{
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    /// <summary>Dvoupísmenný kód jazyka, např. "en".</summary>
    public string Culture { get; set; } = "en";

    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
}
