using System;
using System.Collections.Generic;
using System.Text;

using FilipunkBlog.Domain.Entities;
using FilipunkBlog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FilipunkBlog.Infrastructure.Persistence.Repositories;

public class BlogPostRepository(IDbContextFactory<ApplicationDbContext> factory)
{
    public async Task<List<BlogPost>> GetPublishedAsync(int skip, int take)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.BlogPosts
            .Include(x => x.Category)
            .Include(x => x.BlogPostTags).ThenInclude(x => x.Tag)
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

    public async Task<BlogPost?> GetBySlugAsync(string slug)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.BlogPosts
            .Include(x => x.Category)
            .Include(x => x.BlogPostTags).ThenInclude(x => x.Tag)
            .FirstOrDefaultAsync(x => x.Slug == slug && x.IsPublished);
    }

    public async Task<BlogPost?> GetByIdAsync(Guid id)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.BlogPosts
            .Include(x => x.Category)
            .Include(x => x.BlogPostTags).ThenInclude(x => x.Tag)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<BlogPost>> GetByCategoryAsync(string categorySlug, int skip, int take)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.BlogPosts
            .Include(x => x.Category)
            .Include(x => x.BlogPostTags).ThenInclude(x => x.Tag)
            .Where(x => x.IsPublished && x.Category.Slug == categorySlug)
            .OrderByDescending(x => x.PublishedAt)
            .Skip(skip).Take(take)
            .ToListAsync();
    }

    public async Task<List<BlogPost>> GetByTagAsync(string tagSlug)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.BlogPosts
            .Include(x => x.Category)
            .Include(x => x.BlogPostTags).ThenInclude(x => x.Tag)
            .Where(x => x.IsPublished && x.BlogPostTags.Any(t => t.Tag.Slug == tagSlug))
            .OrderByDescending(x => x.PublishedAt)
            .ToListAsync();
    }

    public async Task<List<BlogPost>> GetAllAsync()
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.BlogPosts
            .Include(x => x.Category)
            .Include(x => x.BlogPostTags).ThenInclude(x => x.Tag)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<BlogPost>> GetMostViewedAsync(int count)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.BlogPosts
            .Include(x => x.Category)
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


}