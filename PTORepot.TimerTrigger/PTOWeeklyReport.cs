using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace PTORepot.TimerTrigger
{
    public class PTOWeeklyReport
    {
        [FunctionName("PTOWeeklyReport")]
        public static async Task Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

        }
    }
}
