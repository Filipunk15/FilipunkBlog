namespace FilipunkBlog.Domain.Entities;

public class BlogPostTag
{
    public Guid BlogPostId { get; set; }
    public BlogPost BlogPost { get; set; } = null!;
    public Guid TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}