using FilipunkBlog.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FilipunkBlog.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<BlogPost> BlogPosts { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<BlogPostTag> BlogPostTags { get; set; }

    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectTag> ProjectTags { get; set; }
    public DbSet<ProjectImage> ProjectImages { get; set; }

    public DbSet<BlogPostTranslation> BlogPostTranslations { get; set; }
    public DbSet<ProjectTranslation> ProjectTranslations { get; set; }



    public DbSet<Comment> Comments { get; set; }

    public DbSet<SiteSettings> SiteSettings { get; set; }

    public DbSet<CvProfile> CvProfiles { get; set; }
    public DbSet<ExperienceItem> ExperienceItems { get; set; }
    public DbSet<Certification> Certifications { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(EntityBase).IsAssignableFrom(entityType.ClrType))
            {
                builder.Entity(entityType.ClrType)
                    .Property(nameof(EntityBase.UpdatedAt))
                    .IsConcurrencyToken(false);
                builder.Entity(entityType.ClrType)
                    .Property(nameof(EntityBase.CreatedAt))
                    .IsConcurrencyToken(false);
            }
        }

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override int SaveChanges()
    {
        ApplyTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ApplyTimestamps();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    /// <summary>Centrálně udržuje <see cref="EntityBase.CreatedAt"/> / <see cref="EntityBase.UpdatedAt"/>.</summary>
    private void ApplyTimestamps()
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<EntityBase>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = now;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    entry.Property(nameof(EntityBase.CreatedAt)).IsModified = false;
                    break;
            }
        }
    }
}