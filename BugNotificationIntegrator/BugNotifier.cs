using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.Generic;
using System.Text;

namespace BugNotifierIntegrator
{
    public static class BugNotifier
    {
        [FunctionName("BugNotifier")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string issue = req.Query["text"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            issue = issue ?? data?.text;

            if(!string.IsNullOrEmpty(issue))
            {
                var result = await SendSlackMessage(issue);
            }

            string responseMessage = string.IsNullOrEmpty(issue)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {issue}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }

        private static async Task<string> SendSlackMessage(string issue)
        {
            using(var client = new HttpClient())
            {
                Dictionary<string,string> messageDictionary = new Dictionary<string, string>
                {
                    { "text", issue }
                };

                string json = JsonConvert.SerializeObject(messageDictionary);
                var payload = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(string.Format("https://hooks.slack.com/services/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"), payload);

                var result  = await response.Content.ReadAsStringAsync();

                return result;
            }
        }
    }
}
