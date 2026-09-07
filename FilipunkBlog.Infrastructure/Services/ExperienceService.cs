using FilipunkBlog.Application.Contracts;
using FilipunkBlog.Application.Models;
using FilipunkBlog.Domain.Entities;
using FilipunkBlog.Infrastructure.Persistence.Repositories;

namespace FilipunkBlog.Infrastructure.Services;

public class ExperienceService(ExperienceRepository repository) : IExperienceService
{
    public async Task<List<ExperienceEditViewModel>> GetAllAsync()
    {
        var items = await repository.GetAllAsync();
        return items.Select(MapForEdit).ToList();
    }

    public async Task<List<ExperienceViewModel>> GetForHomepageAsync()
    {
        var items = await repository.GetAllAsync();
        return items
            .Where(x => x.ShowOnHomepage)
            .Select(x => Map(x, english: CultureContext.IsEnglish))
            .ToList();
    }

    public async Task<List<ExperienceViewModel>> GetForCvAsync()
    {
        var items = await repository.GetAllAsync();
        return items.Select(x => Map(x, english: false)).ToList();
    }

    public async Task<ExperienceEditViewModel?> GetForEditAsync(Guid id)
    {
        var item = await repository.GetByIdAsync(id);
        return item is null ? null : MapForEdit(item);
    }

    public async Task CreateAsync(ExperienceEditViewModel vm)
    {
        var all = await repository.GetAllAsync();
        var item = new ExperienceItem
        {
            SortOrder = all.Count == 0 ? 0 : all.Max(x => x.SortOrder) + 1,
        };
        Apply(vm, item);
        await repository.AddAsync(item);
    }

    public async Task UpdateAsync(ExperienceEditViewModel vm)
    {
        var item = await repository.GetByIdAsync(vm.Id);
        if (item is null) return;
        Apply(vm, item);
        await repository.UpdateAsync(item);
    }

    public async Task DeleteAsync(Guid id) => await repository.DeleteAsync(id);

    public async Task MoveAsync(Guid id, int direction)
    {
        var all = await repository.GetAllAsync();
        var index = all.FindIndex(x => x.Id == id);
        if (index < 0) return;
        var target = index + Math.Sign(direction);
        if (target < 0 || target >= all.Count) return;

        (all[index].SortOrder, all[target].SortOrder) = (all[target].SortOrder, all[index].SortOrder);
        await repository.UpdateAsync(all[index]);
        await repository.UpdateAsync(all[target]);
    }

    private static void Apply(ExperienceEditViewModel vm, ExperienceItem item)
    {
        item.Kind = vm.Kind;
        item.Title = vm.Title.Trim();
        item.TitleEn = Clean(vm.TitleEn);
        item.Organization = vm.Organization.Trim();
        item.FromYear = vm.FromYear;
        item.ToYear = vm.ToYear;
        item.Bullets = vm.Bullets?.Trim() ?? string.Empty;
        item.BulletsEn = Clean(vm.BulletsEn);
        item.ShowOnHomepage = vm.ShowOnHomepage;
        if (vm.SortOrder != 0) item.SortOrder = vm.SortOrder;
    }

    private static ExperienceViewModel Map(ExperienceItem x, bool english) => new()
    {
        Id = x.Id,
        Kind = x.Kind,
        Title = english ? Pick(x.TitleEn, x.Title) : x.Title,
        Organization = x.Organization,
        PeriodLabel = PeriodLabel(x.FromYear, x.ToYear, english),
        Bullets = SplitBullets(english ? Pick(x.BulletsEn, x.Bullets) : x.Bullets),
        ShowOnHomepage = x.ShowOnHomepage,
        SortOrder = x.SortOrder,
    };

    private static ExperienceEditViewModel MapForEdit(ExperienceItem x) => new()
    {
        Id = x.Id,
        Kind = x.Kind,
        Title = x.Title,
        TitleEn = x.TitleEn,
        Organization = x.Organization,
        FromYear = x.FromYear,
        ToYear = x.ToYear,
        Bullets = x.Bullets,
        BulletsEn = x.BulletsEn,
        SortOrder = x.SortOrder,
        ShowOnHomepage = x.ShowOnHomepage,
    };

    private static string PeriodLabel(int from, int? to, bool english)
    {
        if (to is null) return $"{from}–{(english ? "present" : "současnost")}";
        return to == from ? from.ToString() : $"{from}–{to}";
    }

    private static List<string> SplitBullets(string text) =>
        text.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

    private static string Pick(string? preferred, string fallback)
        => string.IsNullOrWhiteSpace(preferred) ? fallback : preferred;

    private static string? Clean(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
