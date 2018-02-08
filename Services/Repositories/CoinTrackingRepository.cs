using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JSONCapital.Common;
using JSONCapital.Common.Extensions;
using JSONCapital.Common.Json.Converters;
using JSONCapital.Common.Options;
using JSONCapital.Data.Models;
using JSONCapital.Services.CoinTracking.Models;
using JSONCapital.Services.Exceptions;
using JSONCapital.Services.Json.Converters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace JSONCapital.Services.Repositories
{
    public class CoinTrackingRepository
    {
        private readonly ILogger _logger;
        private readonly IOptions<CoinTrackingOptions> _options;
        private readonly GetTradesRequest _getTradesRequest;
        private readonly JsonSerializerSettings _jsonSzrSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:JSONCapital.Services.Repositories.CoinTrackingRepository"/> class. 
        /// This parameterless constructor is used for test mocking only!
        /// </summary>
        public CoinTrackingRepository()
        {
            // nothing here - used for testing only
        }

        public CoinTrackingRepository(
            ILogger<CoinTrackingRepository> logger,
            IOptions<CoinTrackingOptions> options,
            GetTradesRequest getTradesRequest,
            JsonSerializerSettings jsonSzrSettings)
        {
            _logger = logger;
            _options = options;
            _getTradesRequest = getTradesRequest;
            _jsonSzrSettings = jsonSzrSettings;
        }

        public virtual async Task<IEnumerable<Trade>> DownloadTradesAsync()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            _logger.LogDebug("Entered download trades function.");

            using (var httpClient = new HttpClient())
            {
                var sbLogMsg = new StringBuilder("Sending CoinTracking API GetTrades request with following data:");
                sbLogMsg.AppendLine();

                var formDataContent = new MultipartFormDataContent();
                foreach (var signableProp in _getTradesRequest.SignableProperties)
                {
                    var key = signableProp.Key.ToLower();
                    var val = signableProp.Value.ToString();
                    formDataContent.Add(new StringContent(val), key);

                    // log kvps that are being sent in request body
                    sbLogMsg.AppendLine($"{key}: {val}");
                }

                // create request
                var request = new HttpRequestMessage(HttpMethod.Post, _options.Value.ApiEndpoint) { Content = formDataContent };

                // add header vals to request
                request.Headers.Add("Key", _getTradesRequest.Key);
                request.Headers.Add("Sign", _getTradesRequest.Sign);

                // log header vals
                sbLogMsg.AppendLine();
                sbLogMsg.AppendLine($"Added the following header values:");
                sbLogMsg.AppendLine($"Key: {_getTradesRequest.Key}");
                sbLogMsg.AppendLine($"Sign: {_getTradesRequest.Sign}");

                _logger.LogTrace(sbLogMsg.ToString());

                var logMsg = "";

                var response = await httpClient.SendAsync(request);

                try
                {
                    _logger.LogInformation(LoggingEvents.WebRequest, null, "API request for trade data sent.");

                    if (response.IsSuccessStatusCode)
                    {
                        var responseTyped = await response.Content.ReadAsAsync<GetTradesResponse>(_jsonSzrSettings);

                        if (responseTyped != null && responseTyped.Success)
                        {
                            _logger.LogInformation(LoggingEvents.WebRequest, null, $"Successfully downloaded {responseTyped.Trades?.Count() ?? 0} trades from the CoinTracking API.");

                            // return the trade data
                            return responseTyped.Trades;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            logMsg = $"Error downloading trades ({response.StatusCode} - {response.ReasonPhrase}): {responseTyped?.Error} - {responseTyped?.ErrorMessage}";
                            _logger.LogWarning(LoggingEvents.WebRequestError, logMsg);
                            throw new CoinTrackingException(logMsg);
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        logMsg = $"Error downloading trades ({response.StatusCode} - {response.ReasonPhrase}): {response.Content.ReadAsStringAsync().Result}";
                        _logger.LogWarning(LoggingEvents.WebRequestError, logMsg);
                        throw new CoinTrackingException(logMsg);
                    }
                }
                catch (AggregateException aggregateException)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    var aggEx = aggregateException.Flatten();
                    logMsg = $"Error downloading trades, {aggEx.Message}: {aggEx.StackTrace}";
                    _logger.LogWarning(LoggingEvents.WebRequestError, logMsg);
                    throw new CoinTrackingException(logMsg);
                }
            }
        }
    }
}
