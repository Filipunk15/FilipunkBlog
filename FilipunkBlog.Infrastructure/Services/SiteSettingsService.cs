using FilipunkBlog.Application.Contracts;
using FilipunkBlog.Application.Models;
using FilipunkBlog.Domain.Entities;
using FilipunkBlog.Infrastructure.Persistence.Repositories;

namespace FilipunkBlog.Infrastructure.Services;

public class SiteSettingsService(SiteSettingsRepository repository) : ISiteSettingsService
{
    public async Task<SiteSettingsViewModel> GetAsync()
    {
        var settings = await repository.GetAsync();
        return new SiteSettingsViewModel
        {
            Status = settings.AvailabilityStatus,
            Note = CultureContext.IsEnglish
                ? (string.IsNullOrWhiteSpace(settings.AvailabilityNoteEn) ? settings.AvailabilityNote : settings.AvailabilityNoteEn)
                : settings.AvailabilityNote,
            NoteCs = settings.AvailabilityNote,
            NoteEn = settings.AvailabilityNoteEn,
        };
    }

    public async Task UpdateAvailabilityAsync(AvailabilityStatus status, string? note, string? noteEn = null)
    {
        var settings = await repository.GetAsync();
        settings.AvailabilityStatus = status;
        settings.AvailabilityNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        settings.AvailabilityNoteEn = string.IsNullOrWhiteSpace(noteEn) ? null : noteEn.Trim();
        await repository.UpdateAsync(settings);
    }
}
