namespace FilipunkBlog.Application.Models;

public class TagViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}