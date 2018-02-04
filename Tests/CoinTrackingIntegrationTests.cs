using System;
using System.Linq;
using JSONCapital.Common.Options;
using JSONCapital.Services.CoinTracking.Models;
using JSONCapital.Services.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

namespace JSONCapital.Tests
{
    public class CoinTrackingIntegrationTests
    {
        [Fact]
        public void ParseMockDataFromCoinTrackingApi()
        {
            var loggerFactory = new LoggerFactory().AddConsole();
            var loggerCTR = loggerFactory.CreateLogger<CoinTrackingRepository>();
            var loggerGTR = loggerFactory.CreateLogger<GetTradesRequest>();

            // configure options with mock server endpoint
            var options = Options.Create<CoinTrackingOptions>(new CoinTrackingOptions(){ ApiEndpoint = "https://a4d0fe25-fcc4-414c-be2d-9dc8008eb23a.mock.pstmn.io/api/v1/getTradesSuccess", ApiPublicKey = "", ApiPrivateKey = "" });

            var getTradesRequest = new GetTradesRequest(loggerGTR, options);

            var coinTrackingRepo = new CoinTrackingRepository(loggerCTR, options, getTradesRequest);

            var trades = coinTrackingRepo.DownloadTradesAsync().Result;

            Assert.NotNull(trades);
            Assert.NotEmpty(trades);
            Assert.True(trades.All(t => t.CoinTrackingTradeID > 0));
        }
    }
}
