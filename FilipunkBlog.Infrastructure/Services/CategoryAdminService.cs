using FilipunkBlog.Application.Contracts;
using FilipunkBlog.Application.Models;
using FilipunkBlog.Domain.Entities;
using FilipunkBlog.Infrastructure.Persistence.Repositories;

namespace FilipunkBlog.Infrastructure.Services;

public class CategoryAdminService(CategoryRepository repository) : ICategoryAdminService
{
    public async Task<List<CategoryViewModel>> GetCategoriesAsync()
    {
        var cats = await repository.GetAllAsync();
        return cats.Select(c => new CategoryViewModel
        {
            Id = c.Id,
            Name = CultureContext.IsEnglish ? (c.NameEn ?? c.Name) : c.Name,
            NameEn = c.NameEn,
            Slug = c.Slug
        }).ToList();
    }

    public async Task CreateCategoryAsync(string name, string slug)
    {
        var cat = new Category { Name = name, Slug = slug };
        await repository.AddAsync(cat);
    }

    public async Task SetNameEnAsync(Guid id, string? nameEn)
        => await repository.SetNameEnAsync(id, nameEn);

    public async Task DeleteCategoryAsync(Guid id)
    {
        var cats = await repository.GetAllAsync();
        var cat = cats.FirstOrDefault(c => c.Id == id);
        if (cat != null) await repository.DeleteAsync(cat);
    }
}
