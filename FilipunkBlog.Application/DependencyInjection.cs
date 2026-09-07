using FilipunkBlog.Application.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace FilipunkBlog.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IMarkdownService, MarkdownService>();
        return services;
    }
}
