using FilipunkBlog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FilipunkBlog.Infrastructure.Persistence.Configurations;

public class ProjectTranslationConfiguration : IEntityTypeConfiguration<ProjectTranslation>
{
    public void Configure(EntityTypeBuilder<ProjectTranslation> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Culture).IsRequired().HasMaxLength(10);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Slug).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Description).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Detail).IsRequired();

        builder.HasIndex(x => new { x.ProjectId, x.Culture }).IsUnique();
        builder.HasIndex(x => new { x.Culture, x.Slug }).IsUnique();

        builder.HasOne(x => x.Project)
            .WithMany(x => x.Translations)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.UpdatedAt).IsConcurrencyToken(false);
        builder.Property(x => x.CreatedAt).IsConcurrencyToken(false);
    }
}
