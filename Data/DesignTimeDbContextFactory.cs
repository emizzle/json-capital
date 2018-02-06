using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.DependencyInjection;
using JSONCapital.Common.Helpers;
using Microsoft.EntityFrameworkCore;

namespace JSONCapital.Data.Services
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../Web")))
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{EnvironmentHelper.GetEnvironmentName()}.json", optional: true, reloadOnChange: true)
                .AddUserSecrets<Common.Options.CoinTrackingOptions>()
                .Build();
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            builder.UseSqlServer(connectionString);
            return new ApplicationDbContext(builder.Options);
        }
    }
}
