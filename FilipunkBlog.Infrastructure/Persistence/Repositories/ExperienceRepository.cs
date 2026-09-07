using FilipunkBlog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FilipunkBlog.Infrastructure.Persistence.Repositories;

public class ExperienceRepository(IDbContextFactory<ApplicationDbContext> factory)
{
    public async Task<List<ExperienceItem>> GetAllAsync()
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.ExperienceItems
            .OrderBy(x => x.SortOrder)
            .ThenByDescending(x => x.FromYear)
            .ToListAsync();
    }

    public async Task<ExperienceItem?> GetByIdAsync(Guid id)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.ExperienceItems.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(ExperienceItem item)
    {
        await using var context = await factory.CreateDbContextAsync();
        context.ExperienceItems.Add(item);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ExperienceItem item)
    {
        await using var context = await factory.CreateDbContextAsync();
        item.UpdatedAt = DateTime.UtcNow;
        context.ExperienceItems.Update(item);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await using var context = await factory.CreateDbContextAsync();
        var item = await context.ExperienceItems.FirstOrDefaultAsync(x => x.Id == id);
        if (item is null) return;
        context.ExperienceItems.Remove(item);
        await context.SaveChangesAsync();
    }
}
