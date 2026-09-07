using FilipunkBlog.Application.Contracts;
using FilipunkBlog.Application.Models;
using FilipunkBlog.Infrastructure.Persistence.Repositories;

namespace FilipunkBlog.Infrastructure.Services;

public class CvProfileService(CvProfileRepository repository) : ICvProfileService
{
    public async Task<CvProfileViewModel> GetAsync()
    {
        var p = await repository.GetAsync();
        return new CvProfileViewModel
        {
            FullName = p.FullName,
            Headline = p.Headline,
            Phone = p.Phone,
            Email = p.Email,
            Address = p.Address,
            GitHubUrl = p.GitHubUrl,
            LinkedInUrl = p.LinkedInUrl,
            WebsiteUrl = p.WebsiteUrl,
            Summary = p.Summary,
        };
    }

    public async Task UpdateAsync(CvProfileViewModel vm)
    {
        var p = await repository.GetAsync();
        p.FullName = vm.FullName.Trim();
        p.Headline = vm.Headline.Trim();
        p.Phone = Clean(vm.Phone);
        p.Email = Clean(vm.Email);
        p.Address = Clean(vm.Address);
        p.GitHubUrl = Clean(vm.GitHubUrl);
        p.LinkedInUrl = Clean(vm.LinkedInUrl);
        p.WebsiteUrl = Clean(vm.WebsiteUrl);
        p.Summary = Clean(vm.Summary);
        await repository.UpdateAsync(p);
    }

    private static string? Clean(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
