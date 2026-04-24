using System;
using System.Collections.Generic;
using System.Text;

using FilipunkBlog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FilipunkBlog.Infrastructure.Persistence.Configurations;

public class BlogPostConfiguration : IEntityTypeConfiguration<BlogPost>
{
    public void Configure(EntityTypeBuilder<BlogPost> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Slug).IsRequired().HasMaxLength(200);
        builder.HasIndex(x => x.Slug).IsUnique();
        builder.Property(x => x.Introduction).IsRequired().HasMaxLength(500);
        builder.Property(x => x.Content).IsRequired();
        builder.Property(x => x.Image).HasMaxLength(500);
        builder.HasOne(x => x.Category)
            .WithMany(x => x.BlogPosts)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
