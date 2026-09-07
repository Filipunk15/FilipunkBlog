using FilipunkBlog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FilipunkBlog.Infrastructure.Persistence.Configurations;

public class CvProfileConfiguration : IEntityTypeConfiguration<CvProfile>
{
    public void Configure(EntityTypeBuilder<CvProfile> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FullName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Headline).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Phone).HasMaxLength(60);
        builder.Property(x => x.Email).HasMaxLength(200);
        builder.Property(x => x.Address).HasMaxLength(300);
        builder.Property(x => x.GitHubUrl).HasMaxLength(300);
        builder.Property(x => x.LinkedInUrl).HasMaxLength(300);
        builder.Property(x => x.WebsiteUrl).HasMaxLength(300);
        builder.Property(x => x.Summary).HasMaxLength(2000);
    }
}
