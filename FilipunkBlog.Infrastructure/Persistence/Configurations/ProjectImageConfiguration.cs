using FilipunkBlog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FilipunkBlog.Infrastructure.Persistence.Configurations;

public class ProjectImageConfiguration : IEntityTypeConfiguration<ProjectImage>
{
    public void Configure(EntityTypeBuilder<ProjectImage> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ImageUrl)
            .IsRequired()
            .HasMaxLength(500);
        builder.HasOne(x => x.Project)
            .WithMany(x => x.Images)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.UpdatedAt).IsConcurrencyToken(false);
        builder.Property(x => x.CreatedAt).IsConcurrencyToken(false);
    }
}