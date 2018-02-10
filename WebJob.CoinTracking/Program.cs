using System;
using System.IO;
using JSONCapital.Common.Helpers;
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
using JSONCapital.Services.CoinTracking.Models;
using JSONCapital.Data.Repositories;
using Newtonsoft.Json;
using JSONCapital.Common.Json.Converters;
using JSONCapital.Services.CoinTracking.Json.Converters;

namespace JSONCapital.WebJob.CoinTracking
{
    public class Program
    {
        public static IConfiguration _Configuration { get; set; }

        static void Main(string[] args)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            var configuration = new JobHostConfiguration();

            BuildConfiguration();

            ConfigureServices(serviceCollection);


            var serviceProvider = serviceCollection.BuildServiceProvider();

            var options = serviceProvider.GetRequiredService<IOptions<ConnectionStringsOptions>>();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            var hostingEnvironment = serviceProvider.GetRequiredService<IHostingEnvironment>();

            if (hostingEnvironment.IsDevelopment())
            {
                configuration.UseDevelopmentSettings();
                loggerFactory.AddDebug();
                loggerFactory.AddConsole(_Configuration.GetSection("Logging"));
            }
            configuration.Queues.MaxPollingInterval = TimeSpan.FromSeconds(10);
            configuration.Queues.BatchSize = 1;
            configuration.UseTimers();
            configuration.DashboardConnectionString = options.Value.WebJobsDashboard;// _Configuration.GetConnectionString("WebJobsDashboard");
            configuration.StorageConnectionString = options.Value.WebJobsStorage;// _Configuration.GetConnectionString("WebJobsStorage");
            configuration.LoggerFactory = loggerFactory;
            configuration.JobActivator = new CustomJobActivator(serviceProvider);

            var host = new JobHost(configuration);
            host.RunAndBlock();
        }

        private static void BuildConfiguration()
        {
            // Optional: Setup your configuration:
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{EnvironmentHelper.GetEnvironmentName()}.json", optional: true, reloadOnChange: true)
				.AddUserSecrets<Program>();

            _Configuration = configuration.Build();
        }

        private static IConfiguration ConfigureServices(IServiceCollection serviceCollection)
        {
            // Setup your container here, just like a asp.net core app

            // option pattern
            serviceCollection.Configure<ConnectionStringsOptions>(_Configuration.GetSection("ConnectionStrings"));
            serviceCollection.Configure<CoinTrackingOptions>(_Configuration.GetSection("CoinTracking"));

            // web job - required to be DI-ed up
            serviceCollection.AddScoped<ScheduledTask>();


            // db context
            serviceCollection.AddDbContext<ApplicationDbContext>(options =>
                                                                 options.UseSqlServer(_Configuration.GetConnectionString("DefaultConnection")));

            // json settings
            serviceCollection.AddScoped<JsonSerializerSettings>((args) =>
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new BooleanConverter());
                settings.Converters.Add(new DateTimeConverter());
                settings.Converters.Add(new GetTradesResponseConverter());
                return settings;
            });

            // repositories
            serviceCollection.AddScoped<ITradesRepository, TradesRepository>();
            serviceCollection.AddScoped<ICoinTrackingRepository, CoinTrackingRepository>();

            // hosting
            serviceCollection.AddSingleton<IHostingEnvironment>((IServiceProvider arg) => new HostingEnvironment() { ContentRootPath = Directory.GetCurrentDirectory(), EnvironmentName = EnvironmentHelper.GetEnvironmentName() });

            // logging
            serviceCollection.AddLogging((builder) => builder.AddConfiguration(_Configuration.GetSection("Logging")));

            // models
			serviceCollection.AddSingleton<GetTradesRequest>();

            return _Configuration;
        }
    }
}
