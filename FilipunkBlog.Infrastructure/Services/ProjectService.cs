using FilipunkBlog.Application;
using FilipunkBlog.Application.Contracts;
using FilipunkBlog.Application.Models;
using FilipunkBlog.Domain.Entities;
using FilipunkBlog.Infrastructure.Persistence;
using FilipunkBlog.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FilipunkBlog.Infrastructure.Services;

public class ProjectService(ProjectRepository repository, ApplicationDbContext context) : IProjectService
{
    public async Task<PagedResult<ProjectViewModel>> GetProjectsAsync(int page = 1, int pageSize = 3)
    {
        var skip = (page - 1) * pageSize;
        var projects = await repository.GetPublishedAsync(skip, pageSize);
        var total = await repository.GetPublishedCountAsync();
        return new PagedResult<ProjectViewModel>
        {
            Items = projects.Select(Map).ToList(),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ProjectViewModel?> GetProjectBySlugAsync(string slug)
    {
        var project = await repository.GetBySlugAsync(slug, CultureContext.Current);
        return project == null ? null : Map(project);
    }

    public async Task<List<ProjectViewModel>> GetProjectsByTagAsync(string tagSlug)
    {
        var projects = await repository.GetByTagAsync(tagSlug);
        return projects.Select(Map).ToList();
    }

    public async Task<List<ProjectViewModel>> GetAllProjectsAsync()
    {
        var projects = await repository.GetAllAsync();
        return projects.Select(MapForAdmin).ToList();
    }

    public async Task<ProjectViewModel?> GetForEditAsync(Guid id)
    {
        var project = await repository.GetByIdAsync(id);
        return project == null ? null : MapForAdmin(project);
    }

    public async Task CreateProjectAsync(ProjectViewModel vm)
    {
        var project = new Project
        {
            Title = vm.Title,
            Slug = vm.Slug,
            Description = vm.Description,
            Detail = vm.Detail,
            StartYear = vm.StartYear,
            EndYear = vm.EndYear,
            GitHubUrl = vm.GitHubUrl,
            LiveDemoUrl = vm.LiveDemoUrl,
            IsFeatured = vm.IsFeatured,
            IsPublished = vm.IsPublished,
            ProjectTags = vm.TagIds.Select(id => new ProjectTag { TagId = id }).ToList(),
            Images = new List<ProjectImage>()
        };

        AddImages(project, vm);
        await repository.AddAsync(project);
        await SaveTranslationsAsync(project.Id, vm.Translations);
    }

    public async Task UpdateProjectAsync(ProjectViewModel vm)
    {
        var project = new Project
        {
            Id = vm.Id,
            Title = vm.Title,
            Slug = vm.Slug,
            Description = vm.Description,
            Detail = vm.Detail,
            StartYear = vm.StartYear,
            EndYear = vm.EndYear,
            GitHubUrl = vm.GitHubUrl,
            LiveDemoUrl = vm.LiveDemoUrl,
            IsFeatured = vm.IsFeatured,
            IsPublished = vm.IsPublished,
            ProjectTags = vm.TagIds.Select(id => new ProjectTag { TagId = id }).ToList(),
            Images = new List<ProjectImage>()
        };

        AddImages(project, vm);
        await repository.UpdateAsync(project);
        await SaveTranslationsAsync(vm.Id, vm.Translations);
    }

    private static void AddImages(Project project, ProjectViewModel vm)
    {
        if (vm.MainImage != null)
            project.Images.Add(new ProjectImage { ImageUrl = vm.MainImage, IsMain = true });

        foreach (var url in vm.Images.Where(x => x != vm.MainImage))
            project.Images.Add(new ProjectImage { ImageUrl = url, IsMain = false });
    }

    private async Task SaveTranslationsAsync(Guid projectId, List<ContentTranslationViewModel> translations)
    {
        foreach (var t in translations)
        {
            if (t.IsEmpty)
            {
                await repository.DeleteTranslationAsync(projectId, t.Culture);
                continue;
            }

            await repository.UpsertTranslationAsync(new ProjectTranslation
            {
                ProjectId = projectId,
                Culture = t.Culture,
                Title = t.Title.Trim(),
                Slug = string.IsNullOrWhiteSpace(t.Slug)
                    ? SlugHelper.Slugify(t.Title)
                    : SlugHelper.Slugify(t.Slug),
                Description = t.Summary?.Trim() ?? string.Empty,
                Detail = t.Body?.Trim() ?? string.Empty,
            });
        }
    }

    public async Task DeleteProjectAsync(Guid id)
    {
        var project = await repository.GetByIdAsync(id);
        if (project != null) await repository.DeleteAsync(project);
    }

    public async Task<List<ProjectViewModel>> SearchProjectsAsync(string query)
    {
        var q = context.Projects
            .Include(x => x.ProjectTags).ThenInclude(x => x.Tag)
            .Include(x => x.Images)
            .Include(x => x.Translations)
            .Where(x => x.IsPublished);

        if (CultureContext.IsEnglish)
        {
            q = q.Where(x =>
                x.Translations.Any(t => t.Culture == "en" &&
                    (t.Title.Contains(query) || t.Description.Contains(query) || t.Detail.Contains(query)))
                || x.Title.Contains(query) || x.Description.Contains(query) || x.Detail.Contains(query));
        }
        else
        {
            q = q.Where(x => x.Title.Contains(query) || x.Description.Contains(query) || x.Detail.Contains(query));
        }

        var projects = await q.OrderByDescending(x => x.CreatedAt).ToListAsync();
        return projects.Select(Map).ToList();
    }

    /// <summary>Mapování pro veřejný web – obsah v aktuálním jazyce, fallback po polích na češtinu.</summary>
    private static ProjectViewModel Map(Project project)
    {
        var tr = CultureContext.IsEnglish
            ? project.Translations.FirstOrDefault(t => t.Culture == "en")
            : null;

        return new ProjectViewModel
        {
            Id = project.Id,
            Title = Pick(tr?.Title, project.Title),
            Slug = string.IsNullOrWhiteSpace(tr?.Slug) ? project.Slug : tr!.Slug,
            SlugCs = project.Slug,
            SlugEn = project.Translations.FirstOrDefault(t => t.Culture == "en") is { Slug: var s } && !string.IsNullOrWhiteSpace(s) ? s : null,
            Description = Pick(tr?.Description, project.Description),
            Detail = Pick(tr?.Detail, project.Detail),
            StartYear = project.StartYear,
            EndYear = project.EndYear,
            GitHubUrl = project.GitHubUrl,
            LiveDemoUrl = project.LiveDemoUrl,
            IsFeatured = project.IsFeatured,
            MainImage = project.Images.FirstOrDefault(x => x.IsMain)?.ImageUrl,
            Images = project.Images.Where(x => !x.IsMain).Select(x => x.ImageUrl).ToList(),
            IsPublished = project.IsPublished,
            Tags = project.ProjectTags.Select(t => CultureContext.IsEnglish ? (t.Tag.NameEn ?? t.Tag.Name) : t.Tag.Name).ToList(),
            TagIds = project.ProjectTags.Select(t => t.TagId).ToList()
        };
    }

    private static ProjectViewModel MapForAdmin(Project project) => new()
    {
        Id = project.Id,
        Title = project.Title,
        Slug = project.Slug,
        Description = project.Description,
        Detail = project.Detail,
        StartYear = project.StartYear,
        EndYear = project.EndYear,
        GitHubUrl = project.GitHubUrl,
        LiveDemoUrl = project.LiveDemoUrl,
        IsFeatured = project.IsFeatured,
        MainImage = project.Images.FirstOrDefault(x => x.IsMain)?.ImageUrl,
        Images = project.Images.Where(x => !x.IsMain).Select(x => x.ImageUrl).ToList(),
        IsPublished = project.IsPublished,
        Tags = project.ProjectTags.Select(t => t.Tag.Name).ToList(),
        TagIds = project.ProjectTags.Select(t => t.TagId).ToList(),
        Translations = project.Translations.Select(t => new ContentTranslationViewModel
        {
            Culture = t.Culture,
            Title = t.Title,
            Slug = t.Slug,
            Summary = t.Description,
            Body = t.Detail,
        }).ToList(),
    };

    private static string Pick(string? preferred, string fallback)
        => string.IsNullOrWhiteSpace(preferred) ? fallback : preferred;
}
