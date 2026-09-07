using FilipunkBlog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FilipunkBlog.Infrastructure.Persistence.Configurations;

public class SiteSettingsConfiguration : IEntityTypeConfiguration<SiteSettings>
{
    public void Configure(EntityTypeBuilder<SiteSettings> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.AvailabilityStatus)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.AvailabilityNote)
            .HasMaxLength(160);

        builder.Property(x => x.AvailabilityNoteEn)
            .HasMaxLength(160);

        builder.Property(x => x.UpdatedAt).IsConcurrencyToken(false);
        builder.Property(x => x.CreatedAt).IsConcurrencyToken(false);
    }
}
