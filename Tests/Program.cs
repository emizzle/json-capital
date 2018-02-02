using System;
using System.IO;
using JSONCapital.Common.Options;
using JSONCapital.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using JSONCapital.Services.CoinTracking.Models;

namespace JSONCapital.Tests
{
    public class Program
    {
        public static IConfiguration _Configuration { get; set; }

        static void Main(string[] args)
        {
            IServiceCollection serviceCollection = new ServiceCollection();

            BuildConfiguration();

            ConfigureServices(serviceCollection);


            var serviceProvider = serviceCollection.BuildServiceProvider();

            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            var hostingEnvironment = serviceProvider.GetRequiredService<IHostingEnvironment>();

            if (hostingEnvironment.IsDevelopment())
            {
                loggerFactory.AddDebug();
                loggerFactory.AddConsole(_Configuration);
            }
        }

        private static void BuildConfiguration()
        {
            // Optional: Setup your configuration:
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{GetEnvironmentName()}.json", optional: true, reloadOnChange: true)
                .AddUserSecrets<Program>();

            _Configuration = configuration.Build();
        }

        private static IConfiguration ConfigureServices(IServiceCollection serviceCollection)
        {
            // Setup your container here, just like a asp.net core app

            // option pattern
            serviceCollection.Configure<CoinTrackingOptions>(_Configuration.GetSection("CoinTracking"));

            serviceCollection.AddSingleton<GetTradesRequest, GetTradesRequest>();

            serviceCollection.AddSingleton<IHostingEnvironment>((IServiceProvider arg) => new HostingEnvironment() { ContentRootPath = Directory.GetCurrentDirectory(), EnvironmentName = GetEnvironmentName() });

            serviceCollection.AddLogging();

            // One more thing - tell azure where your azure connection strings are
            //Environment.SetEnvironmentVariable("AzureWebJobsDashboard", _Configuration.GetConnectionString("WebJobsDashboard"));
            //Environment.SetEnvironmentVariable("AzureWebJobsStorage", _Configuration.GetConnectionString("WebJobsStorage"));

            return _Configuration;
        }

        private static string GetEnvironmentName()
        {
            return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
                              Environment.GetEnvironmentVariable("Hosting:Environment") ??
                              Environment.GetEnvironmentVariable("ASPNET_ENV");
        }

    }
}
