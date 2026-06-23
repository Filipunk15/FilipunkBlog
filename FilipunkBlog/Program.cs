using FilipunkBlog.Application;
using FilipunkBlog.Application.Contracts;
using FilipunkBlog.Components.Account;
using FilipunkBlog.Infrastructure;
using FilipunkBlog.Infrastructure.Persistence;
using FilipunkBlog.Infrastructure.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using FilipunkBlog.Components.Account;

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
app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider()
});

app.MapRazorComponents<FilipunkBlog.Components.App>()
    .AddInteractiveServerRenderMode();

app.MapPost("/Account/LoginPost", async (
    HttpContext httpContext,
    SignInManager<IdentityUser> signInManager,
    [FromForm] string email,
    [FromForm] string password) =>
{
    var result = await signInManager.PasswordSignInAsync(email, password, false, false);
    if (result.Succeeded)
        return Results.Redirect("/admin");
    return Results.Redirect("/Account/Login?error=1");
}).DisableAntiforgery();

app.MapGet("/rss", async (IBlogPostService blogPostService, HttpContext httpContext) =>
{
    var posts = await blogPostService.GetPostsAsync(1, 20);
    var sb = new System.Text.StringBuilder();
    sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
    sb.AppendLine("<rss version=\"2.0\">");
    sb.AppendLine("<channel>");
    sb.AppendLine("<title>Filip — Blog</title>");
    sb.AppendLine("<link>https://filipunk.cz</link>");
    sb.AppendLine("<description>Blog o C# a .NET vývoji</description>");
    sb.AppendLine("<language>cs</language>");

    foreach (var post in posts.Items)
    {
        sb.AppendLine("<item>");
        sb.AppendLine($"<title>{System.Security.SecurityElement.Escape(post.Title)}</title>");
        sb.AppendLine($"<link>https://filipunk.cz/posts/{post.Slug}</link>");
        sb.AppendLine($"<description>{System.Security.SecurityElement.Escape(post.Introduction)}</description>");
        sb.AppendLine($"<pubDate>{post.PublishedAt:R}</pubDate>");
        sb.AppendLine($"<guid>https://filipunk.cz/posts/{post.Slug}</guid>");
        sb.AppendLine("</item>");
    }

    sb.AppendLine("</channel>");
    sb.AppendLine("</rss>");

    httpContext.Response.ContentType = "application/rss+xml; charset=utf-8";
    await httpContext.Response.WriteAsync(sb.ToString());
});


app.MapGet("/sitemap.xml", async (IBlogPostService blogPostService, HttpContext httpContext) =>
{
    var posts = await blogPostService.GetPostsAsync(1, 1000);
    var sb = new System.Text.StringBuilder();
    sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
    sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

    foreach (var url in new[] { "", "posts", "search" })
    {
        sb.AppendLine("<url>");
        sb.AppendLine($"<loc>https://filipunk.cz/{url}</loc>");
        sb.AppendLine("<changefreq>weekly</changefreq>");
        sb.AppendLine("</url>");
    }

    foreach (var post in posts.Items)
    {
        sb.AppendLine("<url>");
        sb.AppendLine($"<loc>https://filipunk.cz/posts/{post.Slug}</loc>");
        sb.AppendLine($"<lastmod>{post.PublishedAt:yyyy-MM-dd}</lastmod>");
        sb.AppendLine("<changefreq>monthly</changefreq>");
        sb.AppendLine("</url>");
    }

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