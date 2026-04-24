using System;
using System.Collections.Generic;
using System.Text;

using FilipunkBlog.Application.Models;

namespace FilipunkBlog.Application.Contracts;

public interface IBlogPostService
{
    Task<PagedResult<BlogPostViewModel>> GetPostsAsync(int page = 1, int pageSize = 10);
    Task<List<BlogPostViewModel>> GetRecentPostsAsync(int count = 3);
    Task<BlogPostViewModel?> GetPostBySlugAsync(string slug);
    Task<List<BlogPostViewModel>> GetPostsByCategoryAsync(string categorySlug, int page = 1, int pageSize = 10);
    Task<List<BlogPostViewModel>> GetPostsByTagAsync(string tagSlug);
    Task<List<BlogPostViewModel>> GetAllPostsAsync();
    Task CreatePostAsync(BlogPostViewModel post);
    Task UpdatePostAsync(BlogPostViewModel post);
    Task DeletePostAsync(Guid id);
    Task<List<BlogPostViewModel>> SearchAsync(string query);
    Task IncrementViewsAsync(Guid id);
    Task<List<BlogPostViewModel>> GetMostViewedAsync(int count = 5);
}
