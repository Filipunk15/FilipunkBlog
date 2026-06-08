using System;
using System.Collections.Generic;
using System.Text;

namespace FilipunkBlog.Domain.Entities
{
    public class Comment : EntityBase
    {
        public Guid BlogPostId { get; set; }
        public BlogPost BlogPost { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
    }
}
