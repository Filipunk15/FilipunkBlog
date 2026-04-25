using System;
using System.Collections.Generic;
using System.Text;
using FilipunkBlog.Application.Models;

namespace FilipunkBlog.Application.Contracts;

public interface ICommentService
{
    Task<List<CommentViewModel>> GetApprovedByPostAsync(Guid postId);
    Task<List<CommentViewModel>> GetPendingAsync();
    Task AddAsync(Guid postId, string name, string content);
    Task ApproveAsync(Guid id);
    Task DeleteAsync(Guid id);
}
