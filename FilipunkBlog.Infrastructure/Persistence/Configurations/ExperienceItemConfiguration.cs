using FilipunkBlog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FilipunkBlog.Infrastructure.Persistence.Configurations;

public class ExperienceItemConfiguration : IEntityTypeConfiguration<ExperienceItem>
{
    public void Configure(EntityTypeBuilder<ExperienceItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Kind)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
        builder.Property(x => x.TitleEn).HasMaxLength(200);
        builder.Property(x => x.Organization).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Bullets).IsRequired();
        builder.Property(x => x.BulletsEn);

        builder.HasIndex(x => x.SortOrder);
    }
}
