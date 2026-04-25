using System;
using System.Collections.Generic;
using System.Text;

namespace FilipunkBlog.Application.Models;

public class CommentViewModel
{
    public Guid Id { get; set; }
    public Guid BlogPostId { get; set; }
    public string BlogPostTitle { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedAtDisplay => CreatedAt.ToString("d. MMMM yyyy");
}
