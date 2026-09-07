using System;
using System.Collections.Generic;
using System.Text;

using FilipunkBlog.Application.Contracts;
using FilipunkBlog.Application.Models;
using FilipunkBlog.Domain.Entities;
using FilipunkBlog.Infrastructure.Persistence.Repositories;

namespace FilipunkBlog.Infrastructure.Services;

public class CommentService(CommentRepository repository, INotificationService notifier) : ICommentService
{
    public async Task<List<CommentViewModel>> GetApprovedByPostAsync(Guid postId)
    {
        var comments = await repository.GetApprovedByPostAsync(postId);
        return comments.Select(Map).ToList();
    }

    public async Task<List<CommentViewModel>> GetPendingAsync()
    {
        var comments = await repository.GetPendingAsync();
        return comments.Select(Map).ToList();
    }

    public async Task AddAsync(Guid postId, string name, string content)
    {
        var comment = new Comment
        {
            BlogPostId = postId,
            Name = name,
            Content = content,
            IsApproved = false
        };
        await repository.AddAsync(comment);

        var saved = await repository.GetByIdAsync(comment.Id);
        await notifier.NotifyNewCommentAsync(name, saved?.BlogPost?.Title ?? "?", content);
    }

    public async Task ApproveAsync(Guid id) => await repository.ApproveAsync(id);
    public async Task DeleteAsync(Guid id) => await repository.DeleteAsync(id);

    private static CommentViewModel Map(Comment c) => new()
    {
        Id = c.Id,
        BlogPostId = c.BlogPostId,
        BlogPostTitle = c.BlogPost?.Title ?? string.Empty,
        Name = c.Name,
        Content = c.Content,
        IsApproved = c.IsApproved,
        CreatedAt = c.CreatedAt
    };
}
