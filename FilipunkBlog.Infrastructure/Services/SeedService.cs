using FilipunkBlog.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FilipunkBlog.Infrastructure.Services;

public class SeedService(
    ApplicationDbContext context,
    UserManager<IdentityUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IConfiguration configuration,
    ILogger<SeedService> logger)
{
    public async Task SeedAsync()
    {
        await context.Database.MigrateAsync();

        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
            logger.LogInformation("Role Admin vytvorena.");
        }

        var adminEmail = configuration["AdminUser:Email"]
            ?? throw new InvalidOperationException("Chybí konfigurace 'AdminUser:Email' (user-secrets / proměnná prostředí).");
        var adminPassword = configuration["AdminUser:Password"]
            ?? throw new InvalidOperationException("Chybí konfigurace 'AdminUser:Password' (user-secrets / proměnná prostředí).");

        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var user = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(user, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Admin");
                logger.LogInformation("Admin user vytvoren: {Email}", adminEmail);
            }
            else
            {
                logger.LogError("Chyba pri vytvareni admin usera: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        if (!await context.Categories.AnyAsync())
        {
            context.Categories.AddRange(
                new Domain.Entities.Category { Name = "C#", Slug = "csharp" },
                new Domain.Entities.Category { Name = "ASP.NET", Slug = "aspnet" },
                new Domain.Entities.Category { Name = "Osobní", Slug = "osobni" }
            );
            await context.SaveChangesAsync();
            logger.LogInformation("Vychozi kategorie vytvoreny.");
        }

        if (!await context.SiteSettings.AnyAsync())
        {
            context.SiteSettings.Add(new Domain.Entities.SiteSettings
            {
                AvailabilityStatus = Domain.Entities.AvailabilityStatus.Available
            });
            await context.SaveChangesAsync();
            logger.LogInformation("Vychozi nastaveni webu vytvoreno.");
        }

        await SeedCvAsync();
    }

    /// <summary>Naplní data životopisu (profil, časová osa, certifikáty), pokud ještě nejsou.</summary>
    private async Task SeedCvAsync()
    {
        if (!await context.CvProfiles.AnyAsync())
        {
            context.CvProfiles.Add(new Domain.Entities.CvProfile
            {
                FullName = "Filip Lafata",
                Headline = "Juniorní vývojář",
                Phone = "+420 604 908 266",
                Email = "filip.lafata.28@seznam.cz",
                Address = "Dvořákova 343, Planá 348 15",
                GitHubUrl = "https://www.github.com/filipunk15",
                LinkedInUrl = "https://www.linkedin.com/in/filip-lafata/",
                WebsiteUrl = "https://filipunk.cz",
            });
            await context.SaveChangesAsync();
            logger.LogInformation("CV profil vytvoren.");
        }

        if (!await context.ExperienceItems.AnyAsync())
        {
            context.ExperienceItems.AddRange(
                new Domain.Entities.ExperienceItem
                {
                    Kind = Domain.Entities.ExperienceKind.Work,
                    Title = "IT Technik – Vývojář interních aplikací",
                    TitleEn = "IT Technician – Internal application developer",
                    Organization = "KDK Automotive Czech",
                    FromYear = 2024,
                    ToYear = null,
                    Bullets =
                        "Programování interních aplikací v jazycích C# (Blazor, MAUI) nebo Javascript (Node.js).\n" +
                        "Práce s databází Microsoft SQL, Informix, Oracle. Práce s Microsoft Azure, Microsoft Office.\n" +
                        "Zpracovávání dat z MES a ERP systému.",
                    BulletsEn =
                        "Building internal applications in C# (Blazor, MAUI) and JavaScript (Node.js).\n" +
                        "Working with Microsoft SQL, Informix and Oracle databases. Working with Microsoft Azure and Microsoft Office.\n" +
                        "Processing data from MES and ERP systems.",
                    SortOrder = 0,
                },
                new Domain.Entities.ExperienceItem
                {
                    Kind = Domain.Entities.ExperienceKind.Work,
                    Title = "IT Stáž",
                    TitleEn = "IT Internship",
                    Organization = "SUSPA CZ",
                    FromYear = 2023,
                    ToYear = 2023,
                    Bullets =
                        "Programování maker v Microsoft Excel, instalace síťové infrastruktury a příprava kabelových rozvodů pro nové kanceláře.\n" +
                        "Instalace a konfigurace čteček čárových kódů, tvorba a správa systémů pro generování čárových kódů.",
                    BulletsEn =
                        "Writing macros in Microsoft Excel, installing network infrastructure and preparing cabling for new offices.\n" +
                        "Installing and configuring barcode scanners, building and maintaining barcode-generation systems.",
                    SortOrder = 1,
                },
                new Domain.Entities.ExperienceItem
                {
                    Kind = Domain.Entities.ExperienceKind.Education,
                    Title = "Střední vzdělání s maturitou – Informační technologie",
                    TitleEn = "Secondary school with a school-leaving exam – Information Technology",
                    Organization = "SPŠ Tachov, Světce 1",
                    FromYear = 2019,
                    ToYear = 2023,
                    Bullets =
                        "Programování a vývoj aplikací v Javě, instalace a správa operačních systémů, systémů i aplikačního programového vybavení.",
                    BulletsEn =
                        "Programming and application development in Java, installing and managing operating systems and application software.",
                    SortOrder = 2,
                }
            );
            await context.SaveChangesAsync();
            logger.LogInformation("CV casova osa vytvorena.");
        }

        if (!await context.Certifications.AnyAsync())
        {
            context.Certifications.Add(new Domain.Entities.Certification
            {
                Name = "Foundational C# with Microsoft",
                Issuer = "freeCodeCamp",
                SortOrder = 0,
            });
            await context.SaveChangesAsync();
            logger.LogInformation("CV certifikaty vytvoreny.");
        }
    }
}