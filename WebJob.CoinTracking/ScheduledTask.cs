using JSONCapital.Common;
using JSONCapital.Data;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using Microsoft.Extensions.Options;
using JSONCapital.Common.Options;
using System.Collections.Generic;
using Services.CoinTracking.Models;
using System.Linq;

namespace JSONCapital.WebJob.CoinTracking
{
    public class ScheduledTask
    {
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IHostingEnvironment _environment;
        private readonly IOptions<CoinTrackingOptions> _options;
        private readonly GetTradesRequest _getTradesRequest;

        public ScheduledTask(
            ILogger<ScheduledTask> logger, 
            ApplicationDbContext dbContext, 
            IHostingEnvironment env, 
            IOptions<CoinTrackingOptions> options,
            GetTradesRequest getTradesRequest)
        {
            _logger = logger;
            _dbContext = dbContext;
            _environment = env;
            _options = options;
            _getTradesRequest = getTradesRequest;
        }

        public async Task DownloadTrades([TimerTrigger("0 0 * * * *", RunOnStartup = true)] TimerInfo timerInfo, TextWriter log)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            log.WriteLine("Download trades has fired");
            _logger.LogInformation(LoggingEvents.InformationalMarker, "Download trades has fired");

            using (var httpClient = new HttpClient())
            {
                var nvc = new List<KeyValuePair<string, string>>(
                    _getTradesRequest.SignableProperties.Select(prop => new KeyValuePair<string, string>(prop.Key, prop.Value.ToString()))
                );
                var req = new HttpRequestMessage(HttpMethod.Post, _options.Value.ApiEndpoint) { Content = new FormUrlEncodedContent(nvc) };
                var response = await httpClient.SendAsync(req);
            }
        }

        //public async Task DoSomethingOnAQueue([QueueTrigger("myqueuename")] int id)
        //{
        //    _usefulClass.DoSomethingAmazing(id);
        //}
    }
}
