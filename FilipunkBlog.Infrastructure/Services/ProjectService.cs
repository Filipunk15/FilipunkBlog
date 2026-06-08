using FilipunkBlog.Application.Contracts;
using FilipunkBlog.Application.Models;
using FilipunkBlog.Infrastructure.Persistence;
using FilipunkBlog.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

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
        var project = await repository.GetBySlugAsync(slug);
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
        return projects.Select(Map).ToList();
    }

    public async Task CreateProjectAsync(ProjectViewModel vm)
    {
        var project = new Domain.Entities.Project
        {
            Title = vm.Title,
            Slug = vm.Slug,
            Description = vm.Description,
            Detail = vm.Detail,
            StartYear = vm.StartYear,
            EndYear = vm.EndYear,
            IsPublished = vm.IsPublished,
            ProjectTags = vm.TagIds.Select(id => new Domain.Entities.ProjectTag { TagId = id }).ToList(),
            Images = new List<Domain.Entities.ProjectImage>()
        };

        if (vm.MainImage != null)
            project.Images.Add(new Domain.Entities.ProjectImage { ImageUrl = vm.MainImage, IsMain = true });

        foreach (var url in vm.Images)
            project.Images.Add(new Domain.Entities.ProjectImage { ImageUrl = url, IsMain = false });

        await repository.AddAsync(project);
    }

    public async Task UpdateProjectAsync(ProjectViewModel vm)
    {
        var project = new Domain.Entities.Project
        {
            Id = vm.Id,
            Title = vm.Title,
            Slug = vm.Slug,
            Description = vm.Description,
            Detail = vm.Detail,
            StartYear = vm.StartYear,
            EndYear = vm.EndYear,
            IsPublished = vm.IsPublished,
            ProjectTags = vm.TagIds.Select(id => new Domain.Entities.ProjectTag { TagId = id }).ToList(),
            Images = new List<Domain.Entities.ProjectImage>()
        };

        if (vm.MainImage != null)
            project.Images.Add(new Domain.Entities.ProjectImage { ImageUrl = vm.MainImage, IsMain = true });

        foreach (var url in vm.Images)
            project.Images.Add(new Domain.Entities.ProjectImage { ImageUrl = url, IsMain = false });

        await repository.UpdateAsync(project);
    }

    public async Task DeleteProjectAsync(Guid id)
    {
        var project = await repository.GetByIdAsync(id);
        if (project != null) await repository.DeleteAsync(project);
    }

    public async Task<List<ProjectViewModel>> SearchProjectsAsync(string query)
    {
        var projects = await context.Projects
            .Include(x => x.ProjectTags).ThenInclude(x => x.Tag)
            .Include(x => x.Images)
            .Where(x => x.IsPublished &&
                (x.Title.Contains(query) ||
                 x.Description.Contains(query) ||
                 x.Detail.Contains(query)))
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
        return projects.Select(Map).ToList();
    }

    private static ProjectViewModel Map(Domain.Entities.Project project) => new()
    {
        Id = project.Id,
        Title = project.Title,
        Slug = project.Slug,
        Description = project.Description,
        Detail = project.Detail,
        StartYear = project.StartYear,
        EndYear = project.EndYear,
        MainImage = project.Images.FirstOrDefault(x => x.IsMain)?.ImageUrl,
        Images = project.Images.Select(x => x.ImageUrl).ToList(),
        IsPublished = project.IsPublished,
        Tags = project.ProjectTags.Select(t => t.Tag.Name).ToList(),
        TagIds = project.ProjectTags.Select(t => t.TagId).ToList()
    };
}