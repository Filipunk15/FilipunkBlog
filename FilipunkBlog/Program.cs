using FilipunkBlog.Application;
using FilipunkBlog.Application.Contracts;
using FilipunkBlog.Components.Account;
using FilipunkBlog.Infrastructure;
using FilipunkBlog.Infrastructure.Persistence;
using FilipunkBlog.Infrastructure.Services;
using FilipunkBlog.Localization;
using FilipunkBlog.Security;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Information)
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Marker třída FilipunkBlog.SharedResource leží vedle Resources/SharedResource.resx,
// takže manifest je "FilipunkBlog.SharedResource" – ResourcesPath se nenastavuje.
builder.Services.AddLocalization();

builder.Services.AddCascadingAuthenticationState();

// Cookies: HttpOnly + SameSite všude, Secure v produkci (v Development běží na http://localhost).
var cookieSecurePolicy = builder.Environment.IsDevelopment()
    ? CookieSecurePolicy.SameAsRequest
    : CookieSecurePolicy.Always;

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = cookieSecurePolicy;
});

// Antiforgery cookie se nesmí zapisovat anonymním návštěvníkům veřejných stránek (viz třída).
{
    var antiforgery = builder.Services.Last(d => d.ServiceType == typeof(IAntiforgery));
    builder.Services.Remove(antiforgery);
    builder.Services.AddSingleton<IAntiforgery>(sp => new CookielessPublicAntiforgery(
        antiforgery.ImplementationType is { } type
            ? (IAntiforgery)ActivatorUtilities.CreateInstance(sp, type)
            : antiforgery.ImplementationFactory!(sp) as IAntiforgery
              ?? throw new InvalidOperationException("IAntiforgery nelze vytvořit.")));
}

// HSTS: půl roku bylo minimum podle auditu; rok je běžný standard (výchozí je jen 30 dní).
builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(365);
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
    .AddIdentityCookies(cookies =>
    {
        cookies.ApplicationCookie?.Configure(o =>
        {
            o.Cookie.HttpOnly = true;
            o.Cookie.SameSite = SameSiteMode.Lax;
            o.Cookie.SecurePolicy = cookieSecurePolicy;
        });
        cookies.ExternalCookie?.Configure(o =>
        {
            o.Cookie.HttpOnly = true;
            o.Cookie.SameSite = SameSiteMode.Lax;
            o.Cookie.SecurePolicy = cookieSecurePolicy;
        });
    });

builder.Services.AddIdentityCore<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();

    var seed = scope.ServiceProvider.GetRequiredService<SeedService>();
    await seed.SeedAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseStatusCodePagesWithReExecute("/Error/{0}");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Bezpečnostní hlavičky pro všechny odpovědi (audit: chybějící hlavičky + neúplná CSP).
app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        var headers = context.Response.Headers;
        var host = context.Request.Host.Value;

        // Žádné inline skripty ani externí domény. Styly povolují inline (atributy style=""),
        // obrázky https: kvůli obrázkům vloženým do článků přes Markdown.
        headers.ContentSecurityPolicy =
            "default-src 'self'; " +
            "script-src 'self'; " +
            "style-src 'self' 'unsafe-inline'; " +
            "img-src 'self' data: blob: https:; " +
            "font-src 'self'; " +
            $"connect-src 'self' ws://{host} wss://{host}; " +
            "object-src 'none'; " +
            "base-uri 'self'; " +
            "form-action 'self'; " +
            "frame-ancestors 'none'";

        headers.XContentTypeOptions = "nosniff";
        headers.XFrameOptions = "DENY";
        headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
        headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=(), payment=(), usb=(), interest-cohort=()";
        headers["Cross-Origin-Opener-Policy"] = "same-origin";
        return Task.CompletedTask;
    });

    await next();
});

// /en prefix → anglická verze; bez prefixu čeština.
// Musí běžet PŘED routingem – explicitní UseRouting níže potlačí automatické vložení na začátek pipeline.
app.UseLanguagePath();

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // app.css/app.js nemají obsahový hash v URL – ať prohlížeč po nasazení
        // vždy ověří aktuálnost (levné 304 díky ETag).
        var name = ctx.File.Name;
        if (name is "app.css" or "app.js")
            ctx.Context.Response.Headers.CacheControl = "no-cache";
    }
});

app.UseRouting();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<FilipunkBlog.Components.App>()
    .AddInteractiveServerRenderMode();

app.MapPost("/Account/LoginPost", async (
    HttpContext httpContext,
    SignInManager<IdentityUser> signInManager,
    [FromForm] string email,
    [FromForm] string password) =>
{
    var result = await signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: true);
    if (result.Succeeded)
        return Results.Redirect("/admin");
    if (result.IsLockedOut)
        return Results.Redirect("/Account/Login?error=locked");
    return Results.Redirect("/Account/Login?error=1");
});

var siteBaseUrl = app.Configuration["Site:BaseUrl"]?.TrimEnd('/') ?? "https://filipunk.cz";

// Spustí async operaci pod danou kulturou (kvůli lokalizovanému mapování v službách).
static async Task<T> UnderCulture<T>(string culture, Func<Task<T>> action)
{
    var original = System.Globalization.CultureInfo.CurrentUICulture;
    System.Globalization.CultureInfo.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo(culture);
    try { return await action(); }
    finally { System.Globalization.CultureInfo.CurrentUICulture = original; }
}

static string RssFeed(string title, string description, string language, string baseUrl, string prefix,
    IEnumerable<FilipunkBlog.Application.Models.BlogPostViewModel> posts)
{
    var sb = new System.Text.StringBuilder();
    sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
    sb.AppendLine("<rss version=\"2.0\">");
    sb.AppendLine("<channel>");
    sb.AppendLine($"<title>{System.Security.SecurityElement.Escape(title)}</title>");
    sb.AppendLine($"<link>{baseUrl}{prefix}</link>");
    sb.AppendLine($"<description>{System.Security.SecurityElement.Escape(description)}</description>");
    sb.AppendLine($"<language>{language}</language>");

    foreach (var post in posts)
    {
        sb.AppendLine("<item>");
        sb.AppendLine($"<title>{System.Security.SecurityElement.Escape(post.Title)}</title>");
        sb.AppendLine($"<link>{baseUrl}{prefix}/posts/{post.Slug}</link>");
        sb.AppendLine($"<description>{System.Security.SecurityElement.Escape(post.Introduction)}</description>");
        sb.AppendLine($"<pubDate>{post.PublishedAt:R}</pubDate>");
        sb.AppendLine($"<guid>{baseUrl}{prefix}/posts/{post.Slug}</guid>");
        sb.AppendLine("</item>");
    }

    sb.AppendLine("</channel>");
    sb.AppendLine("</rss>");
    return sb.ToString();
}

// Feed pro CZ i EN. Jazyk určuje middleware podle prefixu /en (nastaví CurrentUICulture),
// takže /rss = čeština a /en/rss = angličtina (prefix middleware přepíše cestu na /rss).
app.MapGet("/rss", async (IBlogPostService blogPostService, HttpContext httpContext) =>
{
    var isEn = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "en";
    var posts = await blogPostService.GetPostsAsync(1, 20);
    httpContext.Response.ContentType = "application/rss+xml; charset=utf-8";
    await httpContext.Response.WriteAsync(isEn
        ? RssFeed("Filip — Blog", "A blog about C# and .NET development", "en", siteBaseUrl, "/en", posts.Items)
        : RssFeed("Filip — Blog", "Blog o C# a .NET vývoji", "cs", siteBaseUrl, "", posts.Items));
});


app.MapGet("/sitemap.xml", async (
    IBlogPostService blogPostService,
    IProjectService projectService,
    HttpContext httpContext) =>
{
    var postsCs = await blogPostService.GetPostsAsync(1, 1000);
    var projectsCs = await projectService.GetProjectsAsync(1, 1000);
    var postsEn = await UnderCulture("en", () => blogPostService.GetPostsAsync(1, 1000));
    var projectsEn = await UnderCulture("en", () => projectService.GetProjectsAsync(1, 1000));

    var enPostSlug = postsEn.Items.ToDictionary(p => p.Id, p => p.Slug);
    var enProjectSlug = projectsEn.Items.ToDictionary(p => p.Id, p => p.Slug);

    var sb = new System.Text.StringBuilder();
    sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
    sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\" xmlns:xhtml=\"http://www.w3.org/1999/xhtml\">");

    // Vypíše dvojici CZ + EN URL se vzájemnými hreflang odkazy.
    void Pair(string csPath, string enPath, string changefreq, string? lastmod = null)
    {
        var csLoc = $"{siteBaseUrl}/{csPath}".TrimEnd('/');
        var enLoc = $"{siteBaseUrl}/en/{enPath}".TrimEnd('/');
        foreach (var self in new[] { csLoc, enLoc })
        {
            sb.AppendLine("<url>");
            sb.AppendLine($"<loc>{self}</loc>");
            if (lastmod is not null) sb.AppendLine($"<lastmod>{lastmod}</lastmod>");
            sb.AppendLine($"<changefreq>{changefreq}</changefreq>");
            sb.AppendLine($"<xhtml:link rel=\"alternate\" hreflang=\"cs\" href=\"{csLoc}\" />");
            sb.AppendLine($"<xhtml:link rel=\"alternate\" hreflang=\"en\" href=\"{enLoc}\" />");
            sb.AppendLine($"<xhtml:link rel=\"alternate\" hreflang=\"x-default\" href=\"{csLoc}\" />");
            sb.AppendLine("</url>");
        }
    }

    foreach (var path in new[] { "", "posts", "projects", "setup", "search" })
        Pair(path, path, "weekly");
    Pair("ochrana-osobnich-udaju", "privacy", "yearly");

    foreach (var post in postsCs.Items)
        Pair($"posts/{post.Slug}", $"posts/{enPostSlug.GetValueOrDefault(post.Id, post.Slug)}", "monthly", post.PublishedAt?.ToString("yyyy-MM-dd"));

    foreach (var project in projectsCs.Items)
        Pair($"project/{project.Slug}", $"project/{enProjectSlug.GetValueOrDefault(project.Id, project.Slug)}", "monthly", project.PublishedAt?.ToString("yyyy-MM-dd"));

    sb.AppendLine("</urlset>");
    httpContext.Response.ContentType = "application/xml; charset=utf-8";
    await httpContext.Response.WriteAsync(sb.ToString());
});

try
{
    Log.Information("Spouštím aplikaci...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplikace spadla.");
}
finally
{
    Log.CloseAndFlush();
}