using JSONCapital.Common;
using JSONCapital.Data;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace JSONCapital.WebJob.CoinTracking
{
    public class ScheduledTask
    {
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _dbContext;

        public ScheduledTask(ILogger<ScheduledTask> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task DownloadTrades([TimerTrigger("0 0 * * * *", RunOnStartup = true)] TimerInfo timerInfo, TextWriter log)
        {
            _logger.LogInformation(LoggingEvents.InformationalMarker, "Download trades has fired");
        }

        //public async Task DoSomethingOnAQueue([QueueTrigger("myqueuename")] int id)
        //{
        //    _usefulClass.DoSomethingAmazing(id);
        //}
    }
}
