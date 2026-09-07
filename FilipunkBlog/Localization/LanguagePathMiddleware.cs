using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace FilipunkBlog.Localization;

/// <summary>
/// Prefix <c>/en</c> = anglická verze webu. Middleware odřízne prefix do <see cref="HttpRequest.PathBase"/>
/// (takže <c>@page</c> routy i generované odkazy fungují beze změny) a nastaví jazyk požadavku.
/// Bez prefixu = čeština (výchozí).
/// </summary>
public class LanguagePathMiddleware(RequestDelegate next)
{
    public const string EnPrefix = "/en";
    public static readonly string[] Supported = ["cs", "en"];

    public async Task InvokeAsync(HttpContext context)
    {
        string culture = "cs";

        if (context.Request.Path.StartsWithSegments(EnPrefix, StringComparison.OrdinalIgnoreCase, out var remaining))
        {
            context.Request.PathBase = context.Request.PathBase.Add(EnPrefix);
            context.Request.Path = remaining.HasValue ? remaining : "/";
            culture = "en";
        }

        var ci = CultureInfo.GetCultureInfo(culture);
        CultureInfo.CurrentCulture = ci;
        CultureInfo.CurrentUICulture = ci;
        context.Features.Set<IRequestCultureFeature>(
            new RequestCultureFeature(new RequestCulture(ci), provider: null));

        await next(context);
    }
}

public static class LanguagePathMiddlewareExtensions
{
    public static IApplicationBuilder UseLanguagePath(this IApplicationBuilder app)
        => app.UseMiddleware<LanguagePathMiddleware>();
}
