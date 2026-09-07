using FilipunkBlog.Application.Contracts;
using FilipunkBlog.Application.Models;
using FilipunkBlog.Domain.Entities;
using FilipunkBlog.Infrastructure.Persistence.Repositories;

namespace FilipunkBlog.Infrastructure.Services;

public class CertificationService(CertificationRepository repository) : ICertificationService
{
    public async Task<List<CertificationViewModel>> GetAllAsync()
    {
        var items = await repository.GetAllAsync();
        return items.Select(x => new CertificationViewModel
        {
            Id = x.Id,
            Name = x.Name,
            Issuer = x.Issuer,
            Year = x.Year,
            Url = x.Url,
            SortOrder = x.SortOrder,
        }).ToList();
    }

    public async Task CreateAsync(CertificationViewModel vm)
    {
        var all = await repository.GetAllAsync();
        await repository.AddAsync(new Certification
        {
            Name = vm.Name.Trim(),
            Issuer = vm.Issuer.Trim(),
            Year = vm.Year,
            Url = string.IsNullOrWhiteSpace(vm.Url) ? null : vm.Url.Trim(),
            SortOrder = all.Count == 0 ? 0 : all.Max(x => x.SortOrder) + 1,
        });
    }

    public async Task UpdateAsync(CertificationViewModel vm)
    {
        var item = await repository.GetByIdAsync(vm.Id);
        if (item is null) return;
        item.Name = vm.Name.Trim();
        item.Issuer = vm.Issuer.Trim();
        item.Year = vm.Year;
        item.Url = string.IsNullOrWhiteSpace(vm.Url) ? null : vm.Url.Trim();
        await repository.UpdateAsync(item);
    }

    public async Task DeleteAsync(Guid id) => await repository.DeleteAsync(id);
}
