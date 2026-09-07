using FilipunkBlog.Application.Models;
using FilipunkBlog.Domain.Entities;

namespace FilipunkBlog.Application.Contracts;

public interface ISiteSettingsService
{
    Task<SiteSettingsViewModel> GetAsync();
    Task UpdateAvailabilityAsync(AvailabilityStatus status, string? note, string? noteEn = null);
}
