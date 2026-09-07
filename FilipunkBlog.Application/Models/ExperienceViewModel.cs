using FilipunkBlog.Domain.Entities;

namespace FilipunkBlog.Application.Models;

/// <summary>Zkušenost pro zobrazení (homepage / CV) – texty už ve správném jazyce.</summary>
public class ExperienceViewModel
{
    public Guid Id { get; set; }
    public ExperienceKind Kind { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Organization { get; set; } = string.Empty;
    public string PeriodLabel { get; set; } = string.Empty;
    public List<string> Bullets { get; set; } = [];
    public bool ShowOnHomepage { get; set; }
    public int SortOrder { get; set; }
}

/// <summary>Zkušenost pro admin editaci – CZ i EN pole zvlášť.</summary>
public class ExperienceEditViewModel
{
    public Guid Id { get; set; }
    public ExperienceKind Kind { get; set; } = ExperienceKind.Work;
    public string Title { get; set; } = string.Empty;
    public string? TitleEn { get; set; }
    public string Organization { get; set; } = string.Empty;
    public int FromYear { get; set; } = DateTime.Today.Year;
    public int? ToYear { get; set; }
    public string Bullets { get; set; } = string.Empty;
    public string? BulletsEn { get; set; }
    public int SortOrder { get; set; }
    public bool ShowOnHomepage { get; set; } = true;
}
