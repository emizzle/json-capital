using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JSONCapital.Services.CoinTracking.Exceptions;
using JSONCapital.Data.Repositories;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using JSONCapital.Data.Models;

namespace JSONCapital.WebJob.CoinTracking
{
    public class ScheduledTask
    {
        private readonly ILogger _logger;
        private readonly ITradesRepository _tradesRepo;
        private readonly ICoinTrackingRepository _coinTrackingRepo;

        public ScheduledTask(
            ILogger<ScheduledTask> logger,
            ITradesRepository tradesRepo,
            ICoinTrackingRepository coinTrackingRepo)
        {
            _logger = logger;
            _tradesRepo = tradesRepo;
            _coinTrackingRepo = coinTrackingRepo;
        }

        public async Task DownloadTrades([TimerTrigger("0 0 * * * *", RunOnStartup = true)] TimerInfo timerInfo, TextWriter log)
        {
            try
            {
                var coinTrackingTrades = await _coinTrackingRepo.DownloadTradesAsync();

                if (coinTrackingTrades != null && coinTrackingTrades.Any())
                {
                    foreach (var coinTrackingTrade in coinTrackingTrades)
                    {
                        var trade = new Trade()
                        {
                            BuyAmount = coinTrackingTrade.BuyAmount,
                            BuyCurrency = coinTrackingTrade.BuyCurrency,
                            CoinTrackingTradeID = coinTrackingTrade.CoinTrackingTradeID,
                            Comment = coinTrackingTrade.Comment,
                            Exchange = coinTrackingTrade.Exchange,
                            FeeAmount = coinTrackingTrade.FeeAmount,
                            FeeCurrency = coinTrackingTrade.FeeCurrency,
                            Group = coinTrackingTrade.Group,
                            ImportedFrom = coinTrackingTrade.ImportedFrom,
                            ImportedTime = coinTrackingTrade.ImportedTime,
                            ImportedTradeID = coinTrackingTrade.ImportedTradeID,
                            SellAmount = coinTrackingTrade.SellAmount,
                            SellCurrency = coinTrackingTrade.SellCurrency,
                            Time = coinTrackingTrade.Time,
                            TradeTypeString = coinTrackingTrade.TradeTypeString
                        };

                        await _tradesRepo.AddOrUpdateTradeAsync(trade);
                    }
                    await _tradesRepo.SaveChangesAsync();
                }
            }
            catch (CoinTrackingException ctex)
            {
                await log.WriteLineAsync("Error during download trades: " + ctex.Message);
            }
            catch (Exception ex)
            {
                await log.WriteLineAsync("Unknown error during downloading of trades: " + ex.Message);
            }


        }
    }
}
