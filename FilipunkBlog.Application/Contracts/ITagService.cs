using System;
using System.Collections.Generic;
using System.Text;

using FilipunkBlog.Application.Models;

namespace FilipunkBlog.Application.Contracts;

public interface ITagService
{
    Task<List<TagViewModel>> GetTagsAsync();
    Task CreateTagAsync(string name, string slug);
    Task DeleteTagAsync(Guid id);
}
