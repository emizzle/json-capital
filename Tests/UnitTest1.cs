using System;
using JSONCapital.Common.Options;
using JSONCapital.Services.CoinTracking.Models;
using JSONCapital.Services.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

namespace JSONCapital.Tests
{
    public class CoinTracking
    {
        [Fact]
        public void ParseMockDataFromCoinTrackingApi()
        {
            var logger = new LoggerFactory().AddConsole().CreateLogger<CoinTracking>();
            var options = Options.Create<CoinTrackingOptions>(new CoinTrackingOptions(){ ApiEndpoint = "https://a4d0fe25-fcc4-414c-be2d-9dc8008eb23a.mock.pstmn.io", ApiPublicKey = "", ApiPrivateKey = "" });
            var getTradesRequest = new GetTradesRequest(logger, options);

            var coinTrackingRepo = new CoinTrackingRepository(logger, options, getTradesRequest);


        }
    }
}
