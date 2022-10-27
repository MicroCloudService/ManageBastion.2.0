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
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
namespace ManageBastion2
{
    public static class SetInitialRun
    {
        [FunctionName("SetInitialRun")]
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
            config[Constants.VNet] ="";
            config[Constants.VNetId] = "";
            config[Constants.PublicIP] = "";
            config[Constants.PublicIPId] = "";

            log.LogInformation("Settings VNetValue Exists: " + SupportFunctions.EnvironmentVariableExists(config[Constants.VNet]));
            log.LogInformation("Settings Public_IP Exists: " + SupportFunctions.EnvironmentVariableExists(config[Constants.PublicIP]));

            string responseMessage = "";
            responseMessage = $"PublicIP {BastionModel.PublicIP}. VNET : {BastionModel.VNet}. PublicIP_ID {BastionModel.PublicIPId}. VNET_ID : {BastionModel.VNetId}";

            return new OkObjectResult(responseMessage);
        }
    }
}
