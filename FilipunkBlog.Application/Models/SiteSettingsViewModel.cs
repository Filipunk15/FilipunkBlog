using FilipunkBlog.Domain.Entities;

namespace FilipunkBlog.Application.Models;

public class SiteSettingsViewModel
{
    public AvailabilityStatus Status { get; set; } = AvailabilityStatus.Available;

    /// <summary>Poznámka v aktuálním jazyce (s fallbackem na CZ).</summary>
    public string? Note { get; set; }

    /// <summary>Surové hodnoty pro admin editaci.</summary>
    public string? NoteCs { get; set; }
    public string? NoteEn { get; set; }

    public string Label => Status switch
    {
        AvailabilityStatus.Available => "K dispozici pro nové projekty",
        AvailabilityStatus.Busy => "Momentálně vytížen",
        AvailabilityStatus.Unavailable => "Nepřijímám nové projekty",
        _ => string.Empty,
    };

    public string ShortLabel => Status switch
    {
        AvailabilityStatus.Available => "K dispozici",
        AvailabilityStatus.Busy => "Vytížen",
        AvailabilityStatus.Unavailable => "Nedostupný",
        _ => string.Empty,
    };

    /// <summary>Tailwind třída pro barevnou tečku indikátoru.</summary>
    public string DotClass => Status switch
    {
        AvailabilityStatus.Available => "bg-emerald-500",
        AvailabilityStatus.Busy => "bg-amber-500",
        AvailabilityStatus.Unavailable => "bg-gray-500",
        _ => "bg-gray-500",
    };

    public string TextClass => Status switch
    {
        AvailabilityStatus.Available => "text-emerald-400",
        AvailabilityStatus.Busy => "text-amber-400",
        AvailabilityStatus.Unavailable => "text-gray-400",
        _ => "text-gray-400",
    };

    public string BorderClass => Status switch
    {
        AvailabilityStatus.Available => "border-emerald-500/40",
        AvailabilityStatus.Busy => "border-amber-500/40",
        AvailabilityStatus.Unavailable => "border-gray-600/50",
        _ => "border-gray-600/50",
    };

    public string BgClass => Status switch
    {
        AvailabilityStatus.Available => "bg-emerald-500/10",
        AvailabilityStatus.Busy => "bg-amber-500/10",
        AvailabilityStatus.Unavailable => "bg-gray-500/10",
        _ => "bg-gray-500/10",
    };

    public bool ShowPulse => Status == AvailabilityStatus.Available;
}
