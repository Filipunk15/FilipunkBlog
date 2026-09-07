using System;
using System.Collections.Generic;
using System.Text;

using FilipunkBlog.Application.Contracts;
using FilipunkBlog.Infrastructure.Options;
using FilipunkBlog.Infrastructure.Persistence;
using FilipunkBlog.Infrastructure.Persistence.Repositories;
using FilipunkBlog.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FilipunkBlog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddDbContextFactory<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString), ServiceLifetime.Scoped);

        services.AddScoped<BlogPostRepository>();
        services.AddScoped<CategoryRepository>();
        services.AddScoped<TagRepository>();
        services.AddScoped<SeedService>();
        services.AddScoped<CommentRepository>();
        services.AddScoped<ProjectRepository>();
        services.AddScoped<SiteSettingsRepository>();
        services.AddScoped<CvProfileRepository>();
        services.AddScoped<ExperienceRepository>();
        services.AddScoped<CertificationRepository>();


        services.AddScoped<IBlogPostService, BlogPostService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ITagService, TagAdminService>();
        services.AddScoped<ICategoryAdminService, CategoryAdminService>();
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<ISiteSettingsService, SiteSettingsService>();
        services.AddScoped<ICvProfileService, CvProfileService>();
        services.AddScoped<IExperienceService, ExperienceService>();
        services.AddScoped<ICertificationService, CertificationService>();
        services.AddScoped<ICvService, CvService>();
        services.AddScoped<ICvPdfRenderer, CvPdfRenderer>();

        services.Configure<SmtpOptions>(configuration.GetSection(SmtpOptions.SectionName));
        services.Configure<DiscordOptions>(configuration.GetSection(DiscordOptions.SectionName));
        services.Configure<DeepLOptions>(configuration.GetSection(DeepLOptions.SectionName));
        services.AddHttpClient<INotificationService, NotificationService>();
        services.AddHttpClient<ITranslationService, DeepLTranslationService>();

        return services;
    }
}
