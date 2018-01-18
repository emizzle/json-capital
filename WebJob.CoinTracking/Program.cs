using JSONCapital.Data;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace JSONCapital.WebJob.CoinTracking
{
    public class Program
    {
        public static IConfiguration _Configuration { get; set; }

        static void Main(string[] args)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            var configuration = new JobHostConfiguration();
            var options = ConfigureServices(serviceCollection, configuration.IsDevelopment);

            if (configuration.IsDevelopment)
            {
                configuration.UseDevelopmentSettings();
            }
            configuration.Queues.MaxPollingInterval = TimeSpan.FromSeconds(10);
            configuration.Queues.BatchSize = 1;
            configuration.JobActivator = new CustomJobActivator(serviceCollection.BuildServiceProvider());
            configuration.UseTimers();
            configuration.DashboardConnectionString = _Configuration.GetConnectionString("WebJobsDashboard");
            configuration.StorageConnectionString = _Configuration.GetConnectionString("WebJobsStorage");

            var host = new JobHost(configuration);
            host.RunAndBlock();
        }

        private static IConfiguration ConfigureServices(IServiceCollection serviceCollection, bool isDevelopment)
        {
            // Setup your container here, just like a asp.net core app

            // Optional: Setup your configuration:
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.secrets.json", optional: false, reloadOnChange: true);

            if (isDevelopment)
            {
                configuration
                    .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
                    .AddJsonFile("appsettings.Development.secrets.json", optional: false, reloadOnChange: true);
            }

            _Configuration = configuration.Build();
            serviceCollection.Configure<ConnectionStringsOptions>(_Configuration);

            // A silly example of wiring up some class used by the web job:
            //serviceCollection.AddScoped<ISomeInterface, SomeUsefulClass>();
            // Your classes that contain the webjob methods need to be DI-ed up too
            serviceCollection.AddScoped<ScheduledTask, ScheduledTask>();

            serviceCollection.AddScoped<ApplicationDbContext, ApplicationDbContext>();

            // One more thing - tell azure where your azure connection strings are
            //Environment.SetEnvironmentVariable("AzureWebJobsDashboard", _Configuration.GetConnectionString("WebJobsDashboard"));
            //Environment.SetEnvironmentVariable("AzureWebJobsStorage", _Configuration.GetConnectionString("WebJobsStorage"));

            return _Configuration;
        }

    }
}
