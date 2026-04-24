using FilipunkBlog.Application.Contracts;
using FilipunkBlog.Application.Models;
using FilipunkBlog.Domain.Entities;
using FilipunkBlog.Infrastructure.Persistence.Repositories;

namespace FilipunkBlog.Infrastructure.Services;

public class TagAdminService(TagRepository repository) : ITagService
{
    public async Task<List<TagViewModel>> GetTagsAsync()
    {
        var tags = await repository.GetAllAsync();
        return tags.Select(t => new TagViewModel
        {
            Id = t.Id,
            Name = t.Name,
            Slug = t.Slug
        }).ToList();
    }

    public async Task CreateTagAsync(string name, string slug)
    {
        var tag = new Tag { Name = name, Slug = slug };
        await repository.AddAsync(tag);
    }

    public async Task DeleteTagAsync(Guid id)
    {
        var tags = await repository.GetAllAsync();
        var tag = tags.FirstOrDefault(t => t.Id == id);
        if (tag != null) await repository.DeleteAsync(tag);
    }
}