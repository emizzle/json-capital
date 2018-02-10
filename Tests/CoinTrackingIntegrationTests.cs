using System.Linq;
using JSONCapital.Common.Json.Converters;
using JSONCapital.Common.Options;
using JSONCapital.Services.CoinTracking.Json.Converters;
using JSONCapital.Services.CoinTracking.Models;
using JSONCapital.Data.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Xunit;

namespace JSONCapital.Tests
{
    public class CoinTrackingIntegrationTests
    {
        [Fact]
        public void CoinTrackingRepository_DownloadTrades_ReturnsTrades()
        {
            var loggerFactory = new LoggerFactory().AddConsole();
            var loggerCTR = loggerFactory.CreateLogger<CoinTrackingRepository>();
            var loggerGTR = loggerFactory.CreateLogger<GetTradesRequest>();

            // configure options with mock server endpoint
            var options = Options.Create<CoinTrackingOptions>(new CoinTrackingOptions(){ ApiEndpoint = "https://a4d0fe25-fcc4-414c-be2d-9dc8008eb23a.mock.pstmn.io/api/v1/getTradesSuccess", ApiPublicKey = "", ApiPrivateKey = "" });

            var jsonSzrSettings = new JsonSerializerSettings();
            jsonSzrSettings.Converters.Add(new BooleanConverter());
            jsonSzrSettings.Converters.Add(new DateTimeConverter());
            jsonSzrSettings.Converters.Add(new GetTradesResponseConverter());

            var getTradesRequest = new GetTradesRequest(loggerGTR, options, jsonSzrSettings);

            var coinTrackingRepo = new CoinTrackingRepository(loggerCTR, getTradesRequest);

            var trades = coinTrackingRepo.DownloadTradesAsync().Result;

            Assert.NotNull(trades);
            Assert.NotEmpty(trades);
            Assert.True(trades.All(t => t.CoinTrackingTradeID > 0));
        }
    }
}
