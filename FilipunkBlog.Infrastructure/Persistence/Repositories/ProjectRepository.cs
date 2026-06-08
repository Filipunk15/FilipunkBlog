using FilipunkBlog.Domain.Entities;
using FilipunkBlog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FilipunkBlog.Infrastructure.Persistence.Repositories;

public class ProjectRepository(IDbContextFactory<ApplicationDbContext> factory)
{
    public async Task<List<Project>> GetPublishedAsync(int skip, int take)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.Projects
            .Include(x => x.ProjectTags).ThenInclude(x => x.Tag)
            .Include(x => x.Images)
            .Where(x => x.IsPublished)
            .OrderByDescending(x => x.CreatedAt)
            .Skip(skip).Take(take)
            .ToListAsync();
    }

    public async Task<int> GetPublishedCountAsync()
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.Projects.CountAsync(x => x.IsPublished);
    }

    public async Task<Project?> GetBySlugAsync(string slug, bool includeUnpublished = false)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.Projects
            .Include(x => x.ProjectTags).ThenInclude(x => x.Tag)
            .Include(x => x.Images)
            .FirstOrDefaultAsync(x => x.Slug == slug && (includeUnpublished || x.IsPublished));
    }

    public async Task<Project?> GetByIdAsync(Guid id)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.Projects
            .Include(x => x.ProjectTags).ThenInclude(x => x.Tag)
            .Include(x => x.Images)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Project>> GetByTagAsync(string tagSlug)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.Projects
            .Include(x => x.ProjectTags).ThenInclude(x => x.Tag)
            .Include(x => x.Images)
            .Where(x => x.IsPublished && x.ProjectTags.Any(t => t.Tag.Slug == tagSlug))
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Project>> GetAllAsync()
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.Projects
            .Include(x => x.ProjectTags).ThenInclude(x => x.Tag)
            .Include(x => x.Images)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(Project project)
    {
        await using var context = await factory.CreateDbContextAsync();
        await context.Projects.AddAsync(project);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Project project)
    {
        await using var context = await factory.CreateDbContextAsync();

        var existing = await context.Projects
            .Include(x => x.ProjectTags)
            .Include(x => x.Images)
            .FirstOrDefaultAsync(x => x.Id == project.Id);

        if (existing == null) return;

        existing.Title = project.Title;
        existing.Slug = project.Slug;
        existing.Description = project.Description;
        existing.Detail = project.Detail;
        existing.StartYear = project.StartYear;
        existing.EndYear = project.EndYear;
        existing.IsPublished = project.IsPublished;
        existing.UpdatedAt = DateTime.UtcNow;

        // Přímý SQL delete
        await context.ProjectTags
            .Where(x => x.ProjectId == existing.Id)
            .ExecuteDeleteAsync();

        await context.ProjectImages
            .Where(x => x.ProjectId == existing.Id)
            .ExecuteDeleteAsync();

        existing.ProjectTags.Clear();
        existing.Images.Clear();
        context.ChangeTracker.Clear();

        // Znovu attachni projekt
        context.Projects.Attach(existing);

        foreach (var tag in project.ProjectTags)
            context.ProjectTags.Add(new ProjectTag { ProjectId = existing.Id, TagId = tag.TagId });
        foreach (var img in project.Images)
            context.ProjectImages.Add(new ProjectImage { ProjectId = existing.Id, ImageUrl = img.ImageUrl, IsMain = img.IsMain });

        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Project project)
    {
        await using var context = await factory.CreateDbContextAsync();
        context.Projects.Remove(project);
        await context.SaveChangesAsync();
    }
}