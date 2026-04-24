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

        var adminEmail = configuration["AdminUser:Email"] ?? "admin@filipunk.cz";
        var adminPassword = configuration["AdminUser:Password"] ?? "Admin@123456";

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
    }
}