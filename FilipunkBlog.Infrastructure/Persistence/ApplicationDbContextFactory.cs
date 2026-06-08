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
        builder.UseSqlServer("Server=185.150.25.53;Database=FilipunkBlog_DEV;User Id=appjoin;Password=M@minka284415;Encrypt=True;TrustServerCertificate=True;");
        return new ApplicationDbContext(builder.Options);
    }
}
