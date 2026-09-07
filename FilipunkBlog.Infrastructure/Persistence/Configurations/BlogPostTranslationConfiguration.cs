using FilipunkBlog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FilipunkBlog.Infrastructure.Persistence.Configurations;

public class BlogPostTranslationConfiguration : IEntityTypeConfiguration<BlogPostTranslation>
{
    public void Configure(EntityTypeBuilder<BlogPostTranslation> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Culture).IsRequired().HasMaxLength(10);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Slug).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Introduction).IsRequired();
        builder.Property(x => x.Content).IsRequired();

        builder.HasIndex(x => new { x.BlogPostId, x.Culture }).IsUnique();
        builder.HasIndex(x => new { x.Culture, x.Slug }).IsUnique();

        builder.HasOne(x => x.BlogPost)
            .WithMany(x => x.Translations)
            .HasForeignKey(x => x.BlogPostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.UpdatedAt).IsConcurrencyToken(false);
        builder.Property(x => x.CreatedAt).IsConcurrencyToken(false);
    }
}
