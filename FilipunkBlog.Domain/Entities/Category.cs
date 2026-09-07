namespace FilipunkBlog.Domain.Entities;

public class Category : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public string Slug { get; set; } = string.Empty;
    public ICollection<BlogPost> BlogPosts { get; set; } = [];
}