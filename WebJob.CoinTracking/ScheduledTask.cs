using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JSONCapital.Services.Exceptions;
using JSONCapital.Services.Repositories;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

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
                var trades = await _coinTrackingRepo.DownloadTradesAsync();

                if (trades != null && trades.Any())
                {
                    foreach (var trade in trades)
                    {
                        // TODO: Change scheduled task to Upsert trades instead of simply add all.
                        await _tradesRepo.AddTradeAsync(trade);
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
