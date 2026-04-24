using System;
using System.Collections.Generic;
using System.Text;

using FilipunkBlog.Application.Models;

namespace FilipunkBlog.Application.Contracts;

public interface ICategoryService
{
    Task<List<CategoryViewModel>> GetCategoriesAsync();
}
