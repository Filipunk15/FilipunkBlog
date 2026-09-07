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
            Name = CultureContext.IsEnglish ? (t.NameEn ?? t.Name) : t.Name,
            NameEn = t.NameEn,
            Slug = t.Slug
        }).ToList();
    }

    public async Task<TagViewModel> CreateTagAsync(string name, string slug)
    {
        var tag = new Tag { Name = name, Slug = slug };
        await repository.AddAsync(tag);
        return new TagViewModel { Id = tag.Id, Name = tag.Name, Slug = tag.Slug };
    }

    public async Task SetNameEnAsync(Guid id, string? nameEn)
        => await repository.SetNameEnAsync(id, nameEn);

    public async Task DeleteTagAsync(Guid id)
    {
        var tags = await repository.GetAllAsync();
        var tag = tags.FirstOrDefault(t => t.Id == id);
        if (tag != null) await repository.DeleteAsync(tag);
    }
}
