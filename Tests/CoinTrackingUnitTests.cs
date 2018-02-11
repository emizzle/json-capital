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
        public async Task ScheduledTask_DownloadTrades_AssumeTrue()
        {
            // unit test the WebJob.CoinTracking.ScheduledTask
            // mock trades repository - do we need to mock the .insert method?
            // mock cointrackingrepository - mock the .downloadtrades method to return mocked trades



            // ARRANGE
            // setup our data
            var lstDownloadedTradeData = new List<CoinTrackingTrade>()
            {
                new CoinTrackingTrade()
                {
                    CoinTrackingTradeID = 574025,
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
                    CoinTrackingTradeID = 574024,
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
                new CoinTrackingTrade()
                {
                    CoinTrackingTradeID = 574026,
                    BuyAmount = 1f,
                    BuyCurrency = "BTC",
                    SellAmount = null,
                    SellCurrency = null,
                    FeeAmount = null,
                    FeeCurrency = null,
                    TradeType = CoinTrackingTrade.TradeTypeEnum.Gift_Or_Tip__In,
                    Exchange = "Wallet",
                    Group = null,
                    Comment = "This is a gift Deposit",
                    ImportedFrom="job_wallet",
                    Time= DateTime.ParseExact("25/12/2017 12:45:12", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                    ImportedTime = DateTime.ParseExact("26/12/2017 23:45:12", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                    ImportedTradeID = "123gifttest"
                }
            };

            // setup our mock repos to return some fake data in our target method
            var loggerFactory = new LoggerFactory().AddConsole();
            var loggerTR = loggerFactory.CreateLogger<TradesRepository>();
            var loggerST = loggerFactory.CreateLogger<ScheduledTask>();


            var coinTrackingRepo = new Mock<ICoinTrackingRepository>();
            coinTrackingRepo
                .Setup(repo => repo.DownloadTradesAsync())
                .ReturnsAsync(lstDownloadedTradeData);

            var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "ScheduledTask_DownloadTrades_AssumeTrue")
                .Options;
            dbContextOptions.Freeze();

            // Run the test against one instance of the context
            using (var dbContext = new ApplicationDbContext(dbContextOptions))
            {
                var tradesRepo = new TradesRepository(dbContext, loggerTR);

                // create our WebJob.CoinTracking by injecting our mock repository
                var webJob = new ScheduledTask(loggerST, tradesRepo, coinTrackingRepo.Object);

                // ACT - call our method under test
                var result = webJob.SyncTradesAsync(null, null);
            }

            // Use a separate instance of the context to verify correct data was saved to database
            using (var dbContext = new ApplicationDbContext(dbContextOptions))
            {
                var tradesRepo = new TradesRepository(dbContext, loggerTR);

                // ASSERT - we got the result we expected - our fake data has 6 goals we should get this back from the method
                var tradesFromDbContext = await tradesRepo.GetAllTradesAsync();
                Assert.Equal(lstDownloadedTradeData.Count(), tradesFromDbContext.Count());
                Assert.NotNull(await tradesRepo.FindTradeByCoinTrackingTradeIdAsync(lstDownloadedTradeData[0].CoinTrackingTradeID));
                Assert.NotNull(await tradesRepo.FindTradeByCoinTrackingTradeIdAsync(lstDownloadedTradeData[1].CoinTrackingTradeID));
                Assert.True(tradesFromDbContext.All(t => t.TradeID > 0));
                Assert.Equal(Trade.TradeTypeEnum.Gift_Or_Tip__In, (await tradesRepo.FindTradeByCoinTrackingTradeIdAsync(574026))?.TradeType);
            }




        }
    }
}
