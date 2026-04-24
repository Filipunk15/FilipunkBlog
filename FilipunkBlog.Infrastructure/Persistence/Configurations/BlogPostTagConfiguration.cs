using System;
using System.Collections.Generic;
using System.Text;

using FilipunkBlog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FilipunkBlog.Infrastructure.Persistence.Configurations;

public class BlogPostTagConfiguration : IEntityTypeConfiguration<BlogPostTag>
{
    public void Configure(EntityTypeBuilder<BlogPostTag> builder)
    {
        builder.HasKey(x => new { x.BlogPostId, x.TagId });
        builder.HasOne(x => x.BlogPost)
            .WithMany(x => x.BlogPostTags)
            .HasForeignKey(x => x.BlogPostId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Tag)
            .WithMany(x => x.BlogPostTags)
            .HasForeignKey(x => x.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
