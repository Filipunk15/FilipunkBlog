using System;
using System.Collections.Generic;
using System.Text;

using FilipunkBlog.Application.Contracts;
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


        services.AddScoped<IBlogPostService, BlogPostService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ITagService, TagAdminService>();
        services.AddScoped<ICategoryAdminService, CategoryAdminService>();
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IProjectService, ProjectService>();

        return services;
    }
}
