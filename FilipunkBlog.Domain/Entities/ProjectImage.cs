namespace FilipunkBlog.Domain.Entities;

public class ProjectImage : EntityBase
{

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsMain { get; set; }
}