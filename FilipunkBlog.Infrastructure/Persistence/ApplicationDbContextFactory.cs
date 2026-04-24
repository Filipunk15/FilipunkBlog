using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace FilipunkBlog.Infrastructure.Persistence;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        builder.UseSqlServer("Server=Server;Database=Database;User Id=UserId;Password=Paswrt;Encrypt=True;TrustServerCertificate=True;");
        return new ApplicationDbContext(builder.Options);
    }
}
