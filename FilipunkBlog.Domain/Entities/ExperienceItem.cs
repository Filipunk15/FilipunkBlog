namespace FilipunkBlog.Domain.Entities;

/// <summary>
/// Položka časové osy – práce nebo vzdělání. Sdílená mezi sekcí „Zkušenosti" na homepage
/// a životopisem (<c>/cv.pdf</c>). EN pole existují jen kvůli dvojjazyčné homepage.
/// </summary>
public class ExperienceItem : EntityBase
{
    public ExperienceKind Kind { get; set; } = ExperienceKind.Work;

    public string Title { get; set; } = string.Empty;
    public string? TitleEn { get; set; }

    public string Organization { get; set; } = string.Empty;

    public int FromYear { get; set; }
    public int? ToYear { get; set; }

    /// <summary>Odrážky – jeden řádek = jedna odrážka.</summary>
    public string Bullets { get; set; } = string.Empty;
    public string? BulletsEn { get; set; }

    public int SortOrder { get; set; }
    public bool ShowOnHomepage { get; set; } = true;
}
