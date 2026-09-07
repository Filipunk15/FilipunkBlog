using FilipunkBlog.Application.Contracts;
using FilipunkBlog.Application.Models;
using FilipunkBlog.Domain.Entities;

namespace FilipunkBlog.Infrastructure.Services;

public class CvService(
    ICvProfileService profileService,
    IExperienceService experienceService,
    ICertificationService certificationService,
    IProjectService projectService) : ICvService
{
    public async Task<CvDocumentModel> BuildAsync()
    {
        var profile = await profileService.GetAsync();
        var experience = await experienceService.GetForCvAsync();
        var certs = await certificationService.GetAllAsync();
        var projects = await projectService.GetAllProjectsAsync();

        var model = new CvDocumentModel
        {
            FullName = profile.FullName,
            Headline = profile.Headline,
            Summary = profile.Summary,
            Contact = BuildContact(profile),
            Experience = experience
                .Where(x => x.Kind == ExperienceKind.Work)
                .Select(ToEntry).ToList(),
            Education = experience
                .Where(x => x.Kind == ExperienceKind.Education)
                .Select(ToEntry).ToList(),
            Certifications = certs
                .Select(c => new CvCertification(c.Name, c.Issuer, c.Year)).ToList(),
            Projects = projects
                .Where(p => p.IsPublished)
                .OrderByDescending(p => p.IsFeatured)
                .ThenByDescending(p => p.StartYear)
                .ThenBy(p => p.Title)
                .Select(p => new CvProject(p.Title, p.Description)).ToList(),
        };

        return model;
    }

    private static List<CvContactLine> BuildContact(CvProfileViewModel p)
    {
        var lines = new List<CvContactLine>();
        if (!string.IsNullOrWhiteSpace(p.Phone)) lines.Add(new("Telefon", p.Phone!, null));
        if (!string.IsNullOrWhiteSpace(p.Email)) lines.Add(new("E-mail", p.Email!, $"mailto:{p.Email}"));
        if (!string.IsNullOrWhiteSpace(p.Address)) lines.Add(new("Adresa", p.Address!, null));
        if (!string.IsNullOrWhiteSpace(p.WebsiteUrl)) lines.Add(new("Web", Pretty(p.WebsiteUrl!), p.WebsiteUrl));
        if (!string.IsNullOrWhiteSpace(p.GitHubUrl)) lines.Add(new("GitHub", Pretty(p.GitHubUrl!), p.GitHubUrl));
        if (!string.IsNullOrWhiteSpace(p.LinkedInUrl)) lines.Add(new("LinkedIn", Pretty(p.LinkedInUrl!), p.LinkedInUrl));
        return lines;
    }

    private static CvEntry ToEntry(ExperienceViewModel x) => new()
    {
        Title = x.Title,
        Organization = x.Organization,
        PeriodLabel = x.PeriodLabel,
        Bullets = x.Bullets,
    };

    private static string Pretty(string url) =>
        url.Replace("https://", "").Replace("http://", "").TrimEnd('/');
}
