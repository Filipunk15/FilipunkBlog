using System;
using System.Collections.Generic;
using System.Text;

namespace FilipunkBlog.Domain.Entities
{
    public class Project : EntityBase
    {
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
        public int StartYear { get; set; } = DateTime.Today.Year;
        public int? EndYear { get; set; }
        public ICollection<ProjectImage> Images { get; set; } = [];
        public bool IsPublished { get; set; }
        public ICollection<ProjectTag> ProjectTags { get; set; } = [];
    }
}
