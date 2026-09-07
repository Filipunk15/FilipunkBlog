using FilipunkBlog.Application.Contracts;
using FilipunkBlog.Application.Models;
using FilipunkBlog.Infrastructure.Persistence.Repositories;

namespace FilipunkBlog.Infrastructure.Services;

public class CategoryService(CategoryRepository repository) : ICategoryService
{
    public async Task<List<CategoryViewModel>> GetCategoriesAsync()
    {
        var categories = await repository.GetAllAsync();
        return categories.Select(c => new CategoryViewModel
        {
            Id = c.Id,
            Name = CultureContext.IsEnglish ? (c.NameEn ?? c.Name) : c.Name,
            NameEn = c.NameEn,
            Slug = c.Slug
        }).ToList();
    }
}
