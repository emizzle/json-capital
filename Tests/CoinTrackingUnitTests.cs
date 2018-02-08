﻿using System;
using System.Linq;
using JSONCapital.Common.Options;
using JSONCapital.Services.CoinTracking.Models;
using JSONCapital.Services.Json.Converters;
using JSONCapital.Services.Repositories;
using JSONCapital.Common.Json.Converters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Xunit;
using Moq;
using System.Collections.Generic;
using JSONCapital.Data.Models;
using System.Globalization;
using JSONCapital.WebJob.CoinTracking;
using Microsoft.EntityFrameworkCore;
using JSONCapital.Data;
using System.Threading.Tasks;
using JSONCapital.Tests.Extensions;

namespace JSONCapital.Tests
{
    public class CoinTrackingUnitTests
    {
        [Fact]
        public async Task  InsertTrade_AssumeTrue()
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
            var lstTrades = new List<Trade>()
                    {
                        new Trade()
                        {
                            TradeID = 1,
                            BuyAmount = 0.5f,
                            BuyCurrency = "BTC",
                            SellAmount = 900.2f,
                            SellCurrency = "USD",
                            FeeAmount = 4.5f,
                            FeeCurrency = "USD",
                            TradeType = Trade.TradeTypeEnum.Trade,
                            Exchange = "Kraken",
                             Group = null,
                            Comment = "This is a Kraken Trade",
                            ImportedFrom="kraken",
                            Time= DateTime.ParseExact("25/01/2018 03:45:12", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                            ImportedTime = DateTime.ParseExact("26/01/2018 16:45:12", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                            ImportedTradeID = "43250"
                        },
                        new Trade()
                        {
                            TradeID = 2,
                            BuyAmount = 1f,
                            BuyCurrency = "BTC",
                            SellAmount = null,
                            SellCurrency = null,
                            FeeAmount = null,
                            FeeCurrency = null,
                            TradeType = Trade.TradeTypeEnum.Deposit,
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
            var coinTrackingRepo = new Mock<CoinTrackingRepository>();
            coinTrackingRepo
                .Setup(repo => repo.DownloadTradesAsync())
                .ReturnsAsync(lstTrades);


            //var tradesRepo = new Mock<TradesRepository>();
            //tradesRepo.Setup(repo => repo.AddTradeAsync(It.IsAny<Trade>()));
            //tradesRepo.Setup(repo => repo.SaveChangesAsync());
            //tradesRepo.Setup(repo => repo.GetAllTradesAsync()).ReturnsAsync(lstTrades);
			var loggerFactory = new LoggerFactory().AddConsole();
            var loggerTR = loggerFactory.CreateLogger<TradesRepository>();
			var logger = loggerFactory.CreateLogger<ScheduledTask>();

            var mockSet = new Mock<DbSet<Trade>>();

            // sets up EF to mock async queries
            mockSet.SetupAsync<Trade>(lstTrades.AsQueryable());

            var dbContextOptions = new DbContextOptions<ApplicationDbContext>();
            dbContextOptions.Freeze();
            var mockContext = new Mock<ApplicationDbContext>(dbContextOptions); 
            mockContext.Setup(m => m.Trades).Returns(mockSet.Object);

            var tradesRepo = new TradesRepository(mockContext.Object, loggerTR);


            // create our WebJob.CoinTracking by injecting our mock repository
            var webJob = new ScheduledTask(logger, tradesRepo, coinTrackingRepo.Object);

            // ACT - call our method under test
            var result = webJob.DownloadTrades(null, null);

            // ASSERT - we got the result we expected - our fake data has 6 goals we should get this back from the method
            var tradesFromDbContext = await tradesRepo.GetAllTradesAsync();
            Assert.True(tradesFromDbContext.Count() == lstTrades.Count());
            Assert.True(tradesFromDbContext.All(t => t.TradeID > 0));
        }
    }
}