using FilipunkBlog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FilipunkBlog.Infrastructure.Persistence.Repositories;

public class ProjectRepository(IDbContextFactory<ApplicationDbContext> factory)
{
    private static IQueryable<Project> WithRelations(ApplicationDbContext context) =>
        context.Projects
            .Include(x => x.ProjectTags).ThenInclude(x => x.Tag)
            .Include(x => x.Images)
            .Include(x => x.Translations);

    public async Task<List<Project>> GetPublishedAsync(int skip, int take)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await WithRelations(context)
            .Where(x => x.IsPublished)
            .OrderByDescending(x => x.IsFeatured)
            .ThenByDescending(x => x.StartYear)
            .Skip(skip).Take(take)
            .ToListAsync();
    }

    public async Task<int> GetPublishedCountAsync()
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.Projects.CountAsync(x => x.IsPublished);
    }

    /// <param name="culture">"cs" = základní slug; jinak přeložený slug (s fallbackem na základní).</param>
    public async Task<Project?> GetBySlugAsync(string slug, string culture = "cs", bool includeUnpublished = false)
    {
        await using var context = await factory.CreateDbContextAsync();
        var query = WithRelations(context).Where(x => includeUnpublished || x.IsPublished);

        if (culture != "cs")
        {
            return await query.FirstOrDefaultAsync(x =>
                x.Translations.Any(t => t.Culture == culture && t.Slug == slug) || x.Slug == slug);
        }

        return await query.FirstOrDefaultAsync(x => x.Slug == slug);
    }

    public async Task<Project?> GetByIdAsync(Guid id)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await WithRelations(context).FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Project>> GetByTagAsync(string tagSlug)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await WithRelations(context)
            .Where(x => x.IsPublished && x.ProjectTags.Any(t => t.Tag.Slug == tagSlug))
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Project>> GetAllAsync()
    {
        await using var context = await factory.CreateDbContextAsync();
        return await WithRelations(context)
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

        context.ProjectTags.RemoveRange(existing.ProjectTags);
        context.ProjectImages.RemoveRange(existing.Images);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        existing = await context.Projects
            .FirstOrDefaultAsync(x => x.Id == project.Id);

        if (existing == null) return;

        existing.Title = project.Title;
        existing.Slug = project.Slug;
        existing.Description = project.Description;
        existing.Detail = project.Detail;
        existing.StartYear = project.StartYear;
        existing.EndYear = project.EndYear;
        existing.GitHubUrl = project.GitHubUrl;
        existing.LiveDemoUrl = project.LiveDemoUrl;
        existing.IsFeatured = project.IsFeatured;
        existing.IsPublished = project.IsPublished;
        existing.UpdatedAt = DateTime.UtcNow;

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

    /// <summary>Vloží/aktualizuje překlad projektu (upsert podle Culture).</summary>
    public async Task UpsertTranslationAsync(ProjectTranslation translation)
    {
        await using var context = await factory.CreateDbContextAsync();
        var existing = await context.ProjectTranslations
            .FirstOrDefaultAsync(t => t.ProjectId == translation.ProjectId && t.Culture == translation.Culture);

        if (existing == null)
        {
            context.ProjectTranslations.Add(translation);
        }
        else
        {
            existing.Title = translation.Title;
            existing.Slug = translation.Slug;
            existing.Description = translation.Description;
            existing.Detail = translation.Detail;
        }

        await context.SaveChangesAsync();
    }

    public async Task DeleteTranslationAsync(Guid projectId, string culture)
    {
        await using var context = await factory.CreateDbContextAsync();
        var existing = await context.ProjectTranslations
            .FirstOrDefaultAsync(t => t.ProjectId == projectId && t.Culture == culture);
        if (existing != null)
        {
            context.ProjectTranslations.Remove(existing);
            await context.SaveChangesAsync();
        }
    }
}
