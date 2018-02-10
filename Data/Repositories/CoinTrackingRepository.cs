using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JSONCapital.Common;
using JSONCapital.Common.Extensions;
using JSONCapital.Common.Options;
using JSONCapital.Data.Models;
using JSONCapital.Services.CoinTracking.Exceptions;
using JSONCapital.Services.CoinTracking.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace JSONCapital.Data.Repositories
{
    public interface ICoinTrackingRepository
    {
        Task<IEnumerable<CoinTrackingTrade>> DownloadTradesAsync();
    }

    public class CoinTrackingRepository : ICoinTrackingRepository
    {
        private readonly ILogger _logger;
        private readonly GetTradesRequest _getTradesRequest;

        public CoinTrackingRepository(
            ILogger<CoinTrackingRepository> logger,
            GetTradesRequest getTradesRequest)
        {
            _logger = logger;
            _getTradesRequest = getTradesRequest;
        }

        public virtual async Task<IEnumerable<CoinTrackingTrade>> DownloadTradesAsync()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            _logger.LogDebug("Entered download trades function.");

            var getTradesResponse = await _getTradesRequest.SendRequestAsync<GetTradesResponse>();

            if (getTradesResponse != null && getTradesResponse.Success)
            {
                _logger.LogInformation(LoggingEvents.WebRequest, null, $"Successfully downloaded {getTradesResponse.Trades?.Count() ?? 0} trades from the CoinTracking API.");

                // return the trade data
                return getTradesResponse.Trades;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                var logMsg = $"Error downloading trades from CoinTracking API: {getTradesResponse?.Error} - {getTradesResponse?.ErrorMessage}";
                _logger.LogWarning(LoggingEvents.WebRequestError, logMsg);
                throw new CoinTrackingException(logMsg);
            }
        }
    }
}
