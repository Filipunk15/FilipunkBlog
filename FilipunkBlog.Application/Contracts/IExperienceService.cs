using FilipunkBlog.Application.Models;

namespace FilipunkBlog.Application.Contracts;

public interface IExperienceService
{
    /// <summary>Všechny položky (admin) – řazeno podle SortOrder.</summary>
    Task<List<ExperienceEditViewModel>> GetAllAsync();

    /// <summary>Položky pro časovou osu na homepage – jen ShowOnHomepage, texty v aktuálním jazyce.</summary>
    Task<List<ExperienceViewModel>> GetForHomepageAsync();

    /// <summary>Položky pro životopis (vždy česky) – rozdělené na práci a vzdělání se dá filtrovat přes Kind.</summary>
    Task<List<ExperienceViewModel>> GetForCvAsync();

    Task<ExperienceEditViewModel?> GetForEditAsync(Guid id);
    Task CreateAsync(ExperienceEditViewModel vm);
    Task UpdateAsync(ExperienceEditViewModel vm);
    Task DeleteAsync(Guid id);
    Task MoveAsync(Guid id, int direction);
}
