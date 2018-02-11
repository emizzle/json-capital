using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JSONCapital.Services.CoinTracking.Exceptions;
using JSONCapital.Data.Repositories;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using JSONCapital.Data.Models;
using JSONCapital.Common.Extensions;

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

        /// <summary>
        /// Downloads the trades from the CoinTracking API, persists the necessary data in to the DB, then
        /// deletes any data that did not exist in the import (sync operation).
        /// </summary>
        /// <returns>The trades.</returns>
        /// <param name="timerInfo">Timer info. {second} {minute} {hour} {day} {month} {day of the week}, more info at <see cref="https://codehollow.com/2017/02/azure-functions-time-trigger-cron-cheat-sheet/"/></param>
        /// <param name="log">Log.</param>
        public async Task SyncTradesAsync([TimerTrigger("0 */5 * * * *", RunOnStartup = true)] TimerInfo timerInfo, TextWriter log)
        {
            try
            {
                var coinTrackingTrades = await _coinTrackingRepo.DownloadTradesAsync();

                if (coinTrackingTrades != null && coinTrackingTrades.Any())
                {
                    var existingCoinTrackingTradeIds = _tradesRepo.GetAllCoinTrackingTradeIds();

                    foreach (var coinTrackingTrade in coinTrackingTrades)
                    {
                        // convert CoinTrackingTrade to a Trade model we will persist
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
                            TradeType = coinTrackingTrade.TradeType.ToString().ParseEnum<Trade.TradeTypeEnum>()
                        };

                        await _tradesRepo.AddOrUpdateTradeNoSaveAsync(trade);
                    }

                    // save changes to the context
                    await _tradesRepo.SaveChangesAsync();

                    // find all coinTrackingIds that exist in DB but not in downloaded list
                    var coinTrackingTradeIdsToDelete = existingCoinTrackingTradeIds.Except(coinTrackingTrades.Select(ctt => ctt.CoinTrackingTradeID));

                    // delete them from the context
                    await _tradesRepo.DeleteTradesNoSaveAsync(coinTrackingTradeIdsToDelete);

                    // save changes to the context
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
