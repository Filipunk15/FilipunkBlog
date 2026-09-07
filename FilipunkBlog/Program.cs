using FilipunkBlog.Application;
using FilipunkBlog.Application.Contracts;
using FilipunkBlog.Components.Account;
using FilipunkBlog.Infrastructure;
using FilipunkBlog.Infrastructure.Persistence;
using FilipunkBlog.Infrastructure.Services;
using FilipunkBlog.Localization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

// QuestPDF Community licence – zdarma pro jednotlivce a malé firmy (obrat < 1 mil. USD).
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

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

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
    .AddIdentityCookies();

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

    foreach (var post in postsCs.Items)
        Pair($"posts/{post.Slug}", $"posts/{enPostSlug.GetValueOrDefault(post.Id, post.Slug)}", "monthly", post.PublishedAt?.ToString("yyyy-MM-dd"));

    foreach (var project in projectsCs.Items)
        Pair($"project/{project.Slug}", $"project/{enProjectSlug.GetValueOrDefault(project.Id, project.Slug)}", "monthly", project.PublishedAt?.ToString("yyyy-MM-dd"));

    sb.AppendLine("</urlset>");
    httpContext.Response.ContentType = "application/xml; charset=utf-8";
    await httpContext.Response.WriteAsync(sb.ToString());
});

// Životopis – generuje se živě z CMS dat při každém požadavku (žádný „export" krok).
app.MapGet("/cv.pdf", async (ICvService cv, ICvPdfRenderer renderer) =>
{
    var model = await cv.BuildAsync();
    var bytes = renderer.Render(model);
    return Results.File(bytes, "application/pdf", "Filip-Lafata-CV.pdf");
});

// Přesměrování ze starého (rozbitého) odkazu.
app.MapGet("/files/lafata_cv-cz.pdf", () => Results.Redirect("/cv.pdf"));

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