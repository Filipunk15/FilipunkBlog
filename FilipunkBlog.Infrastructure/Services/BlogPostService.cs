using FilipunkBlog.Application.Contracts;
using FilipunkBlog.Application.Models;
using FilipunkBlog.Infrastructure.Persistence;
using FilipunkBlog.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FilipunkBlog.Infrastructure.Services;

public class BlogPostService(BlogPostRepository repository, ApplicationDbContext context) : IBlogPostService
{
    public async Task<PagedResult<BlogPostViewModel>> GetPostsAsync(int page = 1, int pageSize = 10)
    {
        var skip = (page - 1) * pageSize;
        var posts = await repository.GetPublishedAsync(skip, pageSize);
        var total = await repository.GetPublishedCountAsync();
        return new PagedResult<BlogPostViewModel>
        {
            Items = posts.Select(Map).ToList(),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<List<BlogPostViewModel>> GetRecentPostsAsync(int count = 3)
    {
        var posts = await repository.GetPublishedAsync(0, count);
        return posts.Select(Map).ToList();
    }

    public async Task<BlogPostViewModel?> GetPostBySlugAsync(string slug)
    {
        var post = await repository.GetBySlugAsync(slug);
        return post == null ? null : Map(post);
    }

    public async Task<List<BlogPostViewModel>> GetPostsByCategoryAsync(string categorySlug, int page = 1, int pageSize = 10)
    {
        var skip = (page - 1) * pageSize;
        var posts = await repository.GetByCategoryAsync(categorySlug, skip, pageSize);
        return posts.Select(Map).ToList();
    }

    public async Task<List<BlogPostViewModel>> GetPostsByTagAsync(string tagSlug)
    {
        var posts = await repository.GetByTagAsync(tagSlug);
        return posts.Select(Map).ToList();
    }

    private static BlogPostViewModel Map(Domain.Entities.BlogPost post) => new()
    {
        Id = post.Id,
        Title = post.Title,
        Slug = post.Slug,
        Introduction = post.Introduction,
        Content = post.Content,
        Image = post.Image,
        IsPublished = post.IsPublished,
        PublishedAt = post.PublishedAt,
        CategoryName = post.Category.Name,
        CategorySlug = post.Category.Slug,
        Tags = post.BlogPostTags.Select(t => t.Tag.Name).ToList(),
        Views = post.Views,
    };

    public async Task<List<BlogPostViewModel>> GetAllPostsAsync()
    {
        var posts = await repository.GetAllAsync();
        return posts.Select(Map).ToList();
    }

    public async Task CreatePostAsync(BlogPostViewModel vm)
    {
        var post = new Domain.Entities.BlogPost
        {
            Title = vm.Title,
            Slug = vm.Slug,
            Introduction = vm.Introduction,
            Content = vm.Content,
            Image = vm.Image,
            IsPublished = vm.IsPublished,
            PublishedAt = vm.IsPublished ? DateTime.UtcNow : null,
            CategoryId = vm.CategoryId,
            BlogPostTags = vm.TagIds.Select(id => new Domain.Entities.BlogPostTag { TagId = id }).ToList()
        };
        await repository.AddAsync(post);
    }

    public async Task UpdatePostAsync(BlogPostViewModel vm)
    {
        var post = await repository.GetByIdAsync(vm.Id);
        if (post == null) return;
        post.Title = vm.Title;
        post.Slug = vm.Slug;
        post.Introduction = vm.Introduction;
        post.Content = vm.Content;
        post.Image = vm.Image;
        post.IsPublished = vm.IsPublished;
        post.PublishedAt = vm.IsPublished ? post.PublishedAt ?? DateTime.UtcNow : null;
        post.CategoryId = vm.CategoryId;
        post.UpdatedAt = DateTime.UtcNow;
        post.BlogPostTags = vm.TagIds.Select(id => new Domain.Entities.BlogPostTag
        {
            BlogPostId = post.Id,
            TagId = id
        }).ToList();
        await repository.UpdateAsync(post);
    }

    public async Task DeletePostAsync(Guid id)
    {
        var post = await repository.GetByIdAsync(id);
        if (post != null) await repository.DeleteAsync(post);
    }

    public async Task<List<BlogPostViewModel>> SearchAsync(string query)
    {
        var posts = await context.BlogPosts
            .Include(x => x.Category)
            .Include(x => x.BlogPostTags).ThenInclude(x => x.Tag)
            .Where(x => x.IsPublished &&
                (x.Title.Contains(query) ||
                 x.Introduction.Contains(query) ||
                 x.Content.Contains(query)))
            .OrderByDescending(x => x.PublishedAt)
            .ToListAsync();
        return posts.Select(Map).ToList();
    }

    public async Task IncrementViewsAsync(Guid id)
    {
        await repository.IncrementViewsAsync(id);
    }

    public async Task<List<BlogPostViewModel>> GetMostViewedAsync(int count = 5)
    {
        var posts = await repository.GetMostViewedAsync(count);
        return posts.Select(Map).ToList();
    }
}
