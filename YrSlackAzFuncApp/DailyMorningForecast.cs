using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System;

namespace YrSlackAzFuncApp
{
    public static class DailyMorningForecast
    {
        [FunctionName("DailyMorningForecast")]
        public static void Run([TimerTrigger("0 0 6 * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
