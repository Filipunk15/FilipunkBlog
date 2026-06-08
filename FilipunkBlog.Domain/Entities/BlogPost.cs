namespace FilipunkBlog.Domain.Entities;

public class BlogPost : EntityBase
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Introduction { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Image { get; set; }
    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public ICollection<BlogPostTag> BlogPostTags { get; set; } = [];
    public int Views { get; set; }

    public ICollection<Comment> Comments { get; set; } = [];
}