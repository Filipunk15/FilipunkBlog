using System;
using System.Collections.Generic;
using System.Text;

using FilipunkBlog.Application.Models;

namespace FilipunkBlog.Application.Contracts;

public interface ICategoryAdminService
{
    Task<List<CategoryViewModel>> GetCategoriesAsync();
    Task CreateCategoryAsync(string name, string slug);
    Task SetNameEnAsync(Guid id, string? nameEn);
    Task DeleteCategoryAsync(Guid id);
}
