using System;
using System.Collections.Generic;
using System.Text;

using FilipunkBlog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FilipunkBlog.Infrastructure.Persistence.Repositories;

public class CategoryRepository(IDbContextFactory<ApplicationDbContext> factory)
{
    public async Task<List<Category>> GetAllAsync()
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.Categories.OrderBy(x => x.Name).ToListAsync();
    }

    public async Task AddAsync(Category category)
    {
        await using var context = await factory.CreateDbContextAsync();
        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Category category)
    {
        await using var context = await factory.CreateDbContextAsync();
        context.Categories.Update(category);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Category category)
    {
        await using var context = await factory.CreateDbContextAsync();
        var hasposts = await context.BlogPosts.AnyAsync(x => x.CategoryId == category.Id);
        if (hasposts)
            throw new InvalidOperationException("Kategorie obsahuje příspěvky a nelze ji smazat.");
        context.Categories.Remove(category);
        await context.SaveChangesAsync();
    }
}
