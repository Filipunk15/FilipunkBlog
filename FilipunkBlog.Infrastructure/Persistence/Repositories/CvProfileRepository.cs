using FilipunkBlog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FilipunkBlog.Infrastructure.Persistence.Repositories;

public class CvProfileRepository(IDbContextFactory<ApplicationDbContext> factory)
{
    public async Task<CvProfile> GetAsync()
    {
        await using var context = await factory.CreateDbContextAsync();

        var profile = await context.CvProfiles
            .OrderBy(x => x.CreatedAt)
            .FirstOrDefaultAsync();

        if (profile is null)
        {
            profile = new CvProfile { FullName = "Filip Lafata", Headline = "Vývojář" };
            context.CvProfiles.Add(profile);
            await context.SaveChangesAsync();
        }

        return profile;
    }

    public async Task UpdateAsync(CvProfile profile)
    {
        await using var context = await factory.CreateDbContextAsync();
        profile.UpdatedAt = DateTime.UtcNow;
        context.CvProfiles.Update(profile);
        await context.SaveChangesAsync();
    }
}
