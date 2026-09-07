using FilipunkBlog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FilipunkBlog.Infrastructure.Persistence.Repositories;

public class CertificationRepository(IDbContextFactory<ApplicationDbContext> factory)
{
    public async Task<List<Certification>> GetAllAsync()
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.Certifications
            .OrderBy(x => x.SortOrder)
            .ThenByDescending(x => x.Year)
            .ToListAsync();
    }

    public async Task<Certification?> GetByIdAsync(Guid id)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.Certifications.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(Certification item)
    {
        await using var context = await factory.CreateDbContextAsync();
        context.Certifications.Add(item);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Certification item)
    {
        await using var context = await factory.CreateDbContextAsync();
        item.UpdatedAt = DateTime.UtcNow;
        context.Certifications.Update(item);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await using var context = await factory.CreateDbContextAsync();
        var item = await context.Certifications.FirstOrDefaultAsync(x => x.Id == id);
        if (item is null) return;
        context.Certifications.Remove(item);
        await context.SaveChangesAsync();
    }
}
