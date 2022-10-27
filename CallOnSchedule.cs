using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ManageBastion2
{
    public static class CallOnSchedule
    {
        [FunctionName("CallOnSchedule")]
        public static void Run([TimerTrigger("%OnTimerSchedule%")]TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var caller = SharedTurnOn.Run(log, context);
        }
    }
}
