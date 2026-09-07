using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FilipunkBlog.Infrastructure.Persistence;

/// <summary>
/// Používá se jen pro <c>dotnet ef</c> (migrace) v design-time. Connection string se čte
/// z user-secrets webového projektu nebo z proměnné prostředí
/// <c>ConnectionStrings__DefaultConnection</c> – nikdy není natvrdo v kódu.
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    // shodné s <UserSecretsId> ve FilipunkBlog.csproj
    private const string WebProjectUserSecretsId = "aspnet-FilipunkBlog-4ef1069d-d309-46ef-bd2c-1e11afd12042";

    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets(WebProjectUserSecretsId)
            .AddEnvironmentVariables()
            .Build();

        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Chybí connection string 'DefaultConnection'. Nastav ho přes user-secrets " +
                "(dotnet user-secrets set \"ConnectionStrings:DefaultConnection\" \"...\" --project FilipunkBlog) " +
                "nebo přes proměnnou prostředí ConnectionStrings__DefaultConnection.");

        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        builder.UseSqlServer(connectionString);
        return new ApplicationDbContext(builder.Options);
    }
}
