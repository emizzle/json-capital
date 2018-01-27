using System;
using System.IO;
using JSONCapital.Common.Options;
using JSONCapital.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Azure.WebJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.CoinTracking.Models;

namespace JSONCapital.WebJob.CoinTracking
{
    public class Program
    {
        public static IConfiguration _Configuration { get; set; }

        static void Main(string[] args)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            var configuration = new JobHostConfiguration();

            BuildConfiguration(configuration.IsDevelopment);

            ConfigureServices(serviceCollection);


            var serviceProvider = serviceCollection.BuildServiceProvider();

            var options = serviceProvider.GetRequiredService<IOptions<ConnectionStringsOptions>>();

            var hostingEnvironment = serviceProvider.GetRequiredService<IHostingEnvironment>();


            var logger = configuration.LoggerFactory = new LoggerFactory();
            if (hostingEnvironment.IsDevelopment())
            {
                configuration.UseDevelopmentSettings();
                logger.AddDebug();
                logger.AddConsole();
            }
            configuration.Queues.MaxPollingInterval = TimeSpan.FromSeconds(10);
            configuration.Queues.BatchSize = 1;
            configuration.UseTimers();
            configuration.DashboardConnectionString = options.Value.WebJobsDashboard;// _Configuration.GetConnectionString("WebJobsDashboard");
            configuration.StorageConnectionString = options.Value.WebJobsStorage;// _Configuration.GetConnectionString("WebJobsStorage");
            configuration.LoggerFactory = logger;
            configuration.JobActivator = new CustomJobActivator(serviceProvider);

            var host = new JobHost(configuration);
            host.RunAndBlock();
        }

        public static void BuildConfiguration(bool isDevelopment)
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
            serviceCollection.Configure<ConnectionStringsOptions>(_Configuration.GetSection("ConnectionStrings"));
            serviceCollection.Configure<CoinTrackingOptions>(_Configuration.GetSection("CoinTracking"));

            //serviceCollection.AddScoped<ISomeInterface, SomeUsefulClass>();
            // Your classes that contain the webjob methods need to be DI-ed up too
            serviceCollection.AddScoped<ScheduledTask, ScheduledTask>();

            serviceCollection.AddSingleton<GetTradesRequest, GetTradesRequest>();

            serviceCollection.AddDbContext<ApplicationDbContext>(options =>
                                                                 options.UseSqlServer(_Configuration.GetConnectionString("DefaultConnection")));

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
