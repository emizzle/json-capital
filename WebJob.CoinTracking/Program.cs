using JSONCapital.Data;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

namespace JSONCapital.WebJob.CoinTracking
{
    public class Program
    {
        public static IConfiguration _Configuration { get; set; }

        static void Main(string[] args)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            var configuration = new JobHostConfiguration();
            var hostingEnv = configuration.GetService<IHostingEnvironment>();

            BuildConfiguration(hostingEnv);

            ConfigureServices(serviceCollection);


            var serviceProvider = serviceCollection.BuildServiceProvider();

            var options = serviceProvider.GetService<ConnectionStringsOptions>();


            var logger = configuration.LoggerFactory = new LoggerFactory();
            if (configuration.IsDevelopment)
            {
                configuration.UseDevelopmentSettings();
                logger.AddDebug();
                logger.AddConsole();
            }
            configuration.Queues.MaxPollingInterval = TimeSpan.FromSeconds(10);
            configuration.Queues.BatchSize = 1;
            configuration.UseTimers();
            configuration.DashboardConnectionString = options.WebJobsDashboard;// _Configuration.GetConnectionString("WebJobsDashboard");
            configuration.StorageConnectionString = options.WebJobsStorage;// _Configuration.GetConnectionString("WebJobsStorage");
            configuration.LoggerFactory = logger;
            configuration.JobActivator = new CustomJobActivator(serviceProvider);

            var host = new JobHost(configuration);
            host.RunAndBlock();
        }

        public static void BuildConfiguration(IHostingEnvironment hostingEnv)
        {
            // Optional: Setup your configuration:
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.secrets.json", optional: false, reloadOnChange: true);

            //if (isDevelopment)
            //{
            configuration
                .AddJsonFile($"appsettings.{hostingEnv.EnvironmentName}.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{hostingEnv.EnvironmentName}.secrets.json", optional: false, reloadOnChange: true);
            //}

            _Configuration = configuration.Build();
        }

        private static IConfiguration ConfigureServices(IServiceCollection serviceCollection)
        {
            // Setup your container here, just like a asp.net core app

            // option pattern
            serviceCollection.Configure<ConnectionStringsOptions>(_Configuration);

            //serviceCollection.AddScoped<ISomeInterface, SomeUsefulClass>();
            // Your classes that contain the webjob methods need to be DI-ed up too
            serviceCollection.AddScoped<ScheduledTask, ScheduledTask>();

            serviceCollection.AddDbContext<ApplicationDbContext>(options =>
                                                                 options.UseSqlServer(_Configuration.GetConnectionString("DefaultConnection")));

            serviceCollection.AddLogging();

            // One more thing - tell azure where your azure connection strings are
            //Environment.SetEnvironmentVariable("AzureWebJobsDashboard", _Configuration.GetConnectionString("WebJobsDashboard"));
            //Environment.SetEnvironmentVariable("AzureWebJobsStorage", _Configuration.GetConnectionString("WebJobsStorage"));

            return _Configuration;
        }

    }
}
