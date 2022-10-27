using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Configuration;

namespace ManageBastion2
{
    public static class GetAppSettings
    {
        [FunctionName("GetAppSettings")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var config = new ConfigurationBuilder()
        .SetBasePath(context.FunctionDirectory)
        // This gives you access to your application settings 
        // in your local development environment
        // .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
        //.AddJsonFile("appsettings.json")
        // This is what actually gets you the 
        // application settings in Azure
        .AddEnvironmentVariables()
        .Build();

            BastionModel.TenantID = "885f2aa5-368e-4665-a547-5b31239f92ad";
            BastionModel.SubscriptionID = "e9b63d88-2fb5-49f6-8d1d-2ef618a7f43c";
            BastionModel.ResourceGroup = "Testing";
            BastionModel.SPN = "14bcfc5a-4f24-4c00-8443-9be6b62a3ce3";
            BastionModel.SPNKEY = "PUcJjF_BM4xrr3xVY1~5C~UE3zC~xoiP-1";
            BastionModel.FunctionName = "ManageBastion";
            BastionModel.BastionName = "DemoRun";



            // var list = new List<KeyValuePair<string, string>>();
            // list.Add(new KeyValuePair<string, string>("Cat", "1"));
            // list.Add(new KeyValuePair<string, string>("Dog", "2"));
            // list.Add(new KeyValuePair<string, string>("Rabbit", "3"));

            // // Part 2: loop over list and print pairs.


            Authorization.GetAuthorizationToken();
            Bastion.getBastion(config, log).GetAwaiter().GetResult();
            //await AddAppSettings.getApplicationSettings(log);

            //AddAppSettings.BuildAppSettings(log);
            //await AddAppSettings.AddApplicationSettings();


            // JObject data = JsonConvert.DeserializeObject<JObject>(json);
            var  responseMessage = "Bastion Already On!";

            return new OkObjectResult(responseMessage);
        }
    }
}
