namespace FilipunkBlog.Domain.Entities;

public class Tag : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public ICollection<BlogPostTag> BlogPostTags { get; set; } = [];
    public ICollection<ProjectTag> ProjectTags { get; set; } = [];
}