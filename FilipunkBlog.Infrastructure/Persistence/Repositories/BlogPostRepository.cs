using FilipunkBlog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FilipunkBlog.Infrastructure.Persistence.Repositories;

public class BlogPostRepository(IDbContextFactory<ApplicationDbContext> factory)
{
    // Standardní Include pro čtení včetně překladů.
    private static IQueryable<BlogPost> WithRelations(ApplicationDbContext context) =>
        context.BlogPosts
            .Include(x => x.Category)
            .Include(x => x.BlogPostTags).ThenInclude(x => x.Tag)
            .Include(x => x.Translations);

    public async Task<List<BlogPost>> GetPublishedAsync(int skip, int take)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await WithRelations(context)
            .Where(x => x.IsPublished)
            .OrderByDescending(x => x.PublishedAt)
            .Skip(skip).Take(take)
            .ToListAsync();
    }

    public async Task<int> GetPublishedCountAsync()
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.BlogPosts.CountAsync(x => x.IsPublished);
    }

    /// <param name="culture">"cs" = základní slug; "en" = přeložený slug (s fallbackem na základní).</param>
    public async Task<BlogPost?> GetBySlugAsync(string slug, string culture = "cs")
    {
        await using var context = await factory.CreateDbContextAsync();
        var query = WithRelations(context).Where(x => x.IsPublished);

        if (culture != "cs")
        {
            return await query.FirstOrDefaultAsync(x =>
                x.Translations.Any(t => t.Culture == culture && t.Slug == slug) || x.Slug == slug);
        }

        return await query.FirstOrDefaultAsync(x => x.Slug == slug);
    }

    public async Task<BlogPost?> GetByIdAsync(Guid id)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await WithRelations(context).FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<BlogPost>> GetByCategoryAsync(string categorySlug, int skip, int take)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await WithRelations(context)
            .Where(x => x.IsPublished && x.Category.Slug == categorySlug)
            .OrderByDescending(x => x.PublishedAt)
            .Skip(skip).Take(take)
            .ToListAsync();
    }

    public async Task<List<BlogPost>> GetByTagAsync(string tagSlug)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await WithRelations(context)
            .Where(x => x.IsPublished && x.BlogPostTags.Any(t => t.Tag.Slug == tagSlug))
            .OrderByDescending(x => x.PublishedAt)
            .ToListAsync();
    }

    public async Task<List<BlogPost>> GetAllAsync()
    {
        await using var context = await factory.CreateDbContextAsync();
        return await WithRelations(context)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<BlogPost>> GetMostViewedAsync(int count)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.BlogPosts
            .Include(x => x.Category)
            .Include(x => x.Translations)
            .Where(x => x.IsPublished)
            .OrderByDescending(x => x.Views)
            .Take(count)
            .ToListAsync();
    }

    public async Task AddAsync(BlogPost post)
    {
        await using var context = await factory.CreateDbContextAsync();
        await context.BlogPosts.AddAsync(post);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(BlogPost post)
    {
        await using var context = await factory.CreateDbContextAsync();
        context.BlogPosts.Update(post);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(BlogPost post)
    {
        await using var context = await factory.CreateDbContextAsync();
        context.BlogPosts.Remove(post);
        await context.SaveChangesAsync();
    }

    public async Task IncrementViewsAsync(Guid id)
    {
        await using var context = await factory.CreateDbContextAsync();
        var post = await context.BlogPosts.FindAsync(id);
        if (post == null) return;
        post.Views++;
        await context.SaveChangesAsync();
    }

    /// <summary>Vloží/aktualizuje překlad článku (upsert podle Culture).</summary>
    public async Task UpsertTranslationAsync(BlogPostTranslation translation)
    {
        await using var context = await factory.CreateDbContextAsync();
        var existing = await context.BlogPostTranslations
            .FirstOrDefaultAsync(t => t.BlogPostId == translation.BlogPostId && t.Culture == translation.Culture);

        if (existing == null)
        {
            context.BlogPostTranslations.Add(translation);
        }
        else
        {
            existing.Title = translation.Title;
            existing.Slug = translation.Slug;
            existing.Introduction = translation.Introduction;
            existing.Content = translation.Content;
        }

        await context.SaveChangesAsync();
    }

    public async Task DeleteTranslationAsync(Guid blogPostId, string culture)
    {
        await using var context = await factory.CreateDbContextAsync();
        var existing = await context.BlogPostTranslations
            .FirstOrDefaultAsync(t => t.BlogPostId == blogPostId && t.Culture == culture);
        if (existing != null)
        {
            context.BlogPostTranslations.Remove(existing);
            await context.SaveChangesAsync();
        }
    }
}
