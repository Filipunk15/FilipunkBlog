using FilipunkBlog.Application;
using FilipunkBlog.Application.Contracts;
using FilipunkBlog.Application.Models;
using FilipunkBlog.Domain.Entities;
using FilipunkBlog.Infrastructure.Persistence;
using FilipunkBlog.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

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
        var post = await repository.GetBySlugAsync(slug, CultureContext.Current);
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

    public async Task<List<BlogPostViewModel>> GetAllPostsAsync()
    {
        var posts = await repository.GetAllAsync();
        return posts.Select(MapForAdmin).ToList();
    }

    public async Task<BlogPostViewModel?> GetForEditAsync(Guid id)
    {
        var post = await repository.GetByIdAsync(id);
        return post == null ? null : MapForAdmin(post);
    }

    /// <summary>Mapování pro veřejný web – obsah v aktuálním jazyce, s fallbackem na češtinu po polích.</summary>
    private static BlogPostViewModel Map(BlogPost post)
    {
        var tr = CultureContext.IsEnglish
            ? post.Translations.FirstOrDefault(t => t.Culture == "en")
            : null;

        return new BlogPostViewModel
        {
            Id = post.Id,
            Title = Pick(tr?.Title, post.Title),
            Slug = string.IsNullOrWhiteSpace(tr?.Slug) ? post.Slug : tr!.Slug,
            SlugCs = post.Slug,
            SlugEn = post.Translations.FirstOrDefault(t => t.Culture == "en") is { Slug: var s } && !string.IsNullOrWhiteSpace(s) ? s : null,
            Introduction = Pick(tr?.Introduction, post.Introduction),
            Content = Pick(tr?.Content, post.Content),
            Image = post.Image,
            IsPublished = post.IsPublished,
            PublishedAt = post.PublishedAt,
            CategoryName = CultureContext.IsEnglish ? (post.Category.NameEn ?? post.Category.Name) : post.Category.Name,
            CategorySlug = post.Category.Slug,
            CategoryId = post.CategoryId,
            Tags = post.BlogPostTags.Select(t => CultureContext.IsEnglish ? (t.Tag.NameEn ?? t.Tag.Name) : t.Tag.Name).ToList(),
            TagIds = post.BlogPostTags.Select(t => t.TagId).ToList(),
            Views = post.Views,
        };
    }

    /// <summary>Mapování pro admin – vždy základní (české) pole + seznam překladů k editaci.</summary>
    private static BlogPostViewModel MapForAdmin(BlogPost post) => new()
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
        CategoryId = post.CategoryId,
        Tags = post.BlogPostTags.Select(t => t.Tag.Name).ToList(),
        TagIds = post.BlogPostTags.Select(t => t.TagId).ToList(),
        Views = post.Views,
        Translations = post.Translations.Select(t => new ContentTranslationViewModel
        {
            Culture = t.Culture,
            Title = t.Title,
            Slug = t.Slug,
            Summary = t.Introduction,
            Body = t.Content,
        }).ToList(),
    };

    private static string Pick(string? preferred, string fallback)
        => string.IsNullOrWhiteSpace(preferred) ? fallback : preferred;

    public async Task CreatePostAsync(BlogPostViewModel vm)
    {
        var post = new BlogPost
        {
            Title = vm.Title,
            Slug = vm.Slug,
            Introduction = vm.Introduction,
            Content = vm.Content,
            Image = vm.Image,
            IsPublished = vm.IsPublished,
            PublishedAt = vm.IsPublished ? DateTime.UtcNow : null,
            CategoryId = vm.CategoryId,
            BlogPostTags = vm.TagIds.Select(id => new BlogPostTag { TagId = id }).ToList()
        };
        await repository.AddAsync(post);
        await SaveTranslationsAsync(post.Id, vm.Translations);
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
        post.BlogPostTags = vm.TagIds.Select(id => new BlogPostTag
        {
            BlogPostId = post.Id,
            TagId = id
        }).ToList();
        post.Translations = [];
        await repository.UpdateAsync(post);
        await SaveTranslationsAsync(post.Id, vm.Translations);
    }

    private async Task SaveTranslationsAsync(Guid postId, List<ContentTranslationViewModel> translations)
    {
        foreach (var t in translations)
        {
            if (t.IsEmpty)
            {
                await repository.DeleteTranslationAsync(postId, t.Culture);
                continue;
            }

            await repository.UpsertTranslationAsync(new BlogPostTranslation
            {
                BlogPostId = postId,
                Culture = t.Culture,
                Title = t.Title.Trim(),
                Slug = SlugForTranslation(t),
                Introduction = t.Summary?.Trim() ?? string.Empty,
                Content = t.Body?.Trim() ?? string.Empty,
            });
        }
    }

    private static string SlugForTranslation(ContentTranslationViewModel t)
        => string.IsNullOrWhiteSpace(t.Slug)
            ? SlugHelper.Slugify(t.Title)
            : SlugHelper.Slugify(t.Slug);

    public async Task DeletePostAsync(Guid id)
    {
        var post = await repository.GetByIdAsync(id);
        if (post != null) await repository.DeleteAsync(post);
    }

    public async Task<List<BlogPostViewModel>> SearchAsync(string query)
    {
        var q = context.BlogPosts
            .Include(x => x.Category)
            .Include(x => x.BlogPostTags).ThenInclude(x => x.Tag)
            .Include(x => x.Translations)
            .Where(x => x.IsPublished);

        if (CultureContext.IsEnglish)
        {
            q = q.Where(x =>
                x.Translations.Any(t => t.Culture == "en" &&
                    (t.Title.Contains(query) || t.Introduction.Contains(query) || t.Content.Contains(query)))
                || x.Title.Contains(query) || x.Introduction.Contains(query) || x.Content.Contains(query));
        }
        else
        {
            q = q.Where(x => x.Title.Contains(query) || x.Introduction.Contains(query) || x.Content.Contains(query));
        }

        var posts = await q.OrderByDescending(x => x.PublishedAt).ToListAsync();
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
