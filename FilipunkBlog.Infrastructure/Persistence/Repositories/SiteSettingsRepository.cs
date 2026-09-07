using FilipunkBlog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FilipunkBlog.Infrastructure.Persistence.Repositories;

public class SiteSettingsRepository(IDbContextFactory<ApplicationDbContext> factory)
{
    public async Task<SiteSettings> GetAsync()
    {
        await using var context = await factory.CreateDbContextAsync();

        var settings = await context.SiteSettings
            .OrderBy(x => x.CreatedAt)
            .FirstOrDefaultAsync();

        if (settings is null)
        {
            settings = new SiteSettings { AvailabilityStatus = AvailabilityStatus.Available };
            context.SiteSettings.Add(settings);
            await context.SaveChangesAsync();
        }

        return settings;
    }

    public async Task UpdateAsync(SiteSettings settings)
    {
        await using var context = await factory.CreateDbContextAsync();
        settings.UpdatedAt = DateTime.UtcNow;
        context.SiteSettings.Update(settings);
        await context.SaveChangesAsync();
    }
}
