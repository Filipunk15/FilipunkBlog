using FilipunkBlog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FilipunkBlog.Infrastructure.Persistence.Repositories;

public class TagRepository(IDbContextFactory<ApplicationDbContext> factory)
{
    public async Task<List<Tag>> GetAllAsync()
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.Tags.OrderBy(x => x.Name).ToListAsync();
    }

    public async Task AddAsync(Tag tag)
    {
        await using var context = await factory.CreateDbContextAsync();
        await context.Tags.AddAsync(tag);
        await context.SaveChangesAsync();
    }

    public async Task SetNameEnAsync(Guid id, string? nameEn)
    {
        await using var context = await factory.CreateDbContextAsync();
        var tag = await context.Tags.FindAsync(id);
        if (tag == null) return;
        tag.NameEn = string.IsNullOrWhiteSpace(nameEn) ? null : nameEn.Trim();
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Tag tag)
    {
        await using var context = await factory.CreateDbContextAsync();
        context.Tags.Remove(tag);
        await context.SaveChangesAsync();
    }
}