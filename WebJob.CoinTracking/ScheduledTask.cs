using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace WebJob.CoinTracking
{
    public class ScheduledTask
    {
        private readonly ISomeInterface _usefulClass;

        public ScheduledTask(ISomeInterface usefulClass)
        {
            _usefulClass = usefulClass;
        }

        public async Task DoSomethingOnATimer([TimerTrigger("45 * * * * *", RunOnStartup = false)] TimerInfo timerInfo, TextWriter log)
        {
            _usefulClass.MakeACuppa();
        }

        //public async Task DoSomethingOnAQueue([QueueTrigger("myqueuename")] int id)
        //{
        //    _usefulClass.DoSomethingAmazing(id);
        //}
    }
}
