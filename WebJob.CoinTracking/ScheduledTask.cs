﻿using JSONCapital.Common;
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
using JSONCapital.Data.Models;
using JSONCapital.Common.Extensions;

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
            log.WriteLine("Download trades scheduled task has triggered.");
            _logger.LogDebug("Download trades scheduled task has triggered.");

            using (var httpClient = new HttpClient())
            {
                var logMsg = "Sending CoinTracking API GetTrades request with following data:";
                //_logger.LogDebug(logMsg);
                await log.WriteLineAsync(logMsg);

                var formDataContent = new MultipartFormDataContent();
                foreach (var signableProp in _getTradesRequest.SignableProperties)
                {
                    var key = signableProp.Key.ToLower();
                    var val = signableProp.Value.ToString();
                    formDataContent.Add(new StringContent(val), key);

                    // log kvps that are being sent in request body
                    //_logger.LogDebug($"{key}: {val}");
                    await log.WriteLineAsync($"{key}: {val}");
                }
                var request = new HttpRequestMessage(HttpMethod.Post, _options.Value.ApiEndpoint) { Content = formDataContent };
                request.Headers.Add("Key", _getTradesRequest.Key);
                request.Headers.Add("Sign", _getTradesRequest.Sign);

                // log header vals
                //_logger.LogDebug($"Key: {_getTradesRequest.Key}");
                //_logger.LogDebug($"Sign: {_getTradesRequest.Sign}");
                await log.WriteLineAsync($"Key: {_getTradesRequest.Key}");
                await log.WriteLineAsync($"Sign: {_getTradesRequest.Sign}");



                var response = httpClient.SendAsync(request).Result;

                try
                {
                    await log.WriteLineAsync("API request for trade data sent.");
                    _logger.LogInformation(LoggingEvents.WebRequest, null, "API request for trade data sent.");

                    if (response.IsSuccessStatusCode)
                    {
                        var responseTyped = response.Content.ReadAsAsync<GetTradesResponse>().Result;

                        if (responseTyped != null && responseTyped.Success)
                        {
                            await log.WriteLineAsync("Received a successful trade data response.");
                            _logger.LogInformation(LoggingEvents.WebRequest, null, "Received a successful trade data response.");

                            // persist trade data in DB

                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            logMsg = $"Error downloading trades ({response.StatusCode} - {response.ReasonPhrase}): {responseTyped?.Error} - {responseTyped?.ErrorMessage}";
                            await log.WriteLineAsync(logMsg);
                            _logger.LogCritical(LoggingEvents.WebRequestError, logMsg);
                        }
                    }
                    else
                    {

                        Console.ForegroundColor = ConsoleColor.Red;
                        logMsg = $"Error downloading trades ({response.StatusCode} - {response.ReasonPhrase}): {response.Content.ReadAsStringAsync().Result}";
                        await log.WriteLineAsync(logMsg);
                        _logger.LogCritical(LoggingEvents.WebRequestError, logMsg);
                    }
                }
                catch (AggregateException aggregateException)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    var aggEx = aggregateException.Flatten();
                    logMsg = $"Error downloading trades, {aggEx.Message}: {aggEx.StackTrace}";
                    await log.WriteLineAsync(logMsg);
                    _logger.LogCritical(LoggingEvents.WebRequestError, logMsg);
                }
            }
        }

        //public async Task DoSomethingOnAQueue([QueueTrigger("myqueuename")] int id)
        //{
        //    _usefulClass.DoSomethingAmazing(id);
        //}
    }
}
