using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using JSONCapital.Data;
using JSONCapital.Data.Models;
using JSONCapital.Data.Repositories;
using JSONCapital.Services.CoinTracking.Models;
using JSONCapital.Tests.Extensions;
using JSONCapital.WebJob.CoinTracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace JSONCapital.Tests
{
    public class CoinTrackingUnitTests
    {
        [Fact]
        public async Task InsertTrade_AssumeTrue()
        {

            //var loggerCTR = loggerFactory.CreateLogger<TradesRepository>();

            //// configure options with mock server endpoint
            //var options = Options.Create<CoinTrackingOptions>(new CoinTrackingOptions() { ApiEndpoint = "https://a4d0fe25-fcc4-414c-be2d-9dc8008eb23a.mock.pstmn.io/api/v1/getTradesSuccess", ApiPublicKey = "", ApiPrivateKey = "" });

            //var jsonSzrSettings = new JsonSerializerSettings();
            //jsonSzrSettings.Converters.Add(new BooleanConverter());
            //jsonSzrSettings.Converters.Add(new DateTimeConverter());
            //jsonSzrSettings.Converters.Add(new GetTradesResponseConverter());

            //var getTradesRequest = new GetTradesRequest(loggerGTR, options);

            //var coinTrackingRepo1 = new TradesRepository(loggerCTR, options, getTradesRequest, jsonSzrSettings);

            //var trades = coinTrackingRepo1.DownloadTradesAsync().Result;

            //Assert.NotNull(trades);
            //Assert.NotEmpty(trades);
            //Assert.True(trades.All(t => t.CoinTrackingTradeID > 0));




            // unit test the WebJob.CoinTracking.ScheduledTask
            // mock trades repository - do we need to mock the .insert method?
            // mock cointrackingrepository - mock the .downloadtrades method to return mocked trades



            // ARRANGE
            // setup our data
            var lstTrades = new List<CoinTrackingTrade>()
                    {
                new CoinTrackingTrade()
                        {
                            BuyAmount = 0.5f,
                            BuyCurrency = "BTC",
                            SellAmount = 900.2f,
                            SellCurrency = "USD",
                            FeeAmount = 4.5f,
                            FeeCurrency = "USD",
                    TradeType = CoinTrackingTrade.TradeTypeEnum.Trade,
                            Exchange = "Kraken",
                             Group = null,
                            Comment = "This is a Kraken Trade",
                            ImportedFrom="kraken",
                            Time= DateTime.ParseExact("25/01/2018 03:45:12", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                            ImportedTime = DateTime.ParseExact("26/01/2018 16:45:12", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                            ImportedTradeID = "43250"
                        },
                new CoinTrackingTrade()
                        {
                            BuyAmount = 1f,
                            BuyCurrency = "BTC",
                            SellAmount = null,
                            SellCurrency = null,
                            FeeAmount = null,
                            FeeCurrency = null,
                    TradeType = CoinTrackingTrade.TradeTypeEnum.Deposit,
                            Exchange = "Bittrex",
                             Group = "My Bittrex Deposits",
                            Comment = "This is my Bittrex Deposit",
                            ImportedFrom="job_bittrex",
                            Time= DateTime.ParseExact("25/12/2017 12:45:12", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                            ImportedTime = DateTime.ParseExact("26/12/2017 23:45:12", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                            ImportedTradeID = "fe9ois82msma91d821a"
                        },
            };

            // setup our mock repos to return some fake data in our target method
            var loggerFactory = new LoggerFactory().AddConsole();
            var loggerTR = loggerFactory.CreateLogger<TradesRepository>();
            var loggerST = loggerFactory.CreateLogger<ScheduledTask>();
            //var loggerCTR = loggerFactory.CreateLogger<CoinTrackingRepository>();


            var coinTrackingRepo = new Mock<ICoinTrackingRepository>();
            coinTrackingRepo
                .Setup(repo => repo.DownloadTradesAsync())
                .ReturnsAsync(lstTrades);

            var mockSet = new Mock<DbSet<Trade>>();

            // sets up EF to mock async queries
            mockSet.SetupAsync<Trade>();

            var dbContextOptions = new DbContextOptions<ApplicationDbContext>();
            dbContextOptions.Freeze();
            var mockContext = new Mock<ApplicationDbContext>(dbContextOptions);
            mockContext.Setup(m => m.Trades).Returns(mockSet.Object);

            var tradesRepo = new TradesRepository(mockContext.Object, loggerTR);


            // create our WebJob.CoinTracking by injecting our mock repository
            var webJob = new ScheduledTask(loggerST, tradesRepo, coinTrackingRepo.Object);

            // ACT - call our method under test
            var result = webJob.DownloadTrades(null, null);

            // ASSERT - we got the result we expected - our fake data has 6 goals we should get this back from the method
            var tradesFromDbContext = await tradesRepo.GetAllTradesAsync();
            Assert.True(tradesFromDbContext.Count() == lstTrades.Count());
            Assert.True(tradesFromDbContext.All(t => t.TradeID > 0));
        }
    }
}
