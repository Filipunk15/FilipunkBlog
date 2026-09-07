namespace FilipunkBlog.Application.Models;

public class TagViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public string Slug { get; set; } = string.Empty;
}