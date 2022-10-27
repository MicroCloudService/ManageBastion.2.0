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

using Microsoft.Extensions.Configuration;

namespace ManageBastion2
{
    public static class CreateBastion
    {
        [FunctionName("CreateBastion")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            string responseMessage = "";

            log.LogInformation("C# HTTP trigger function processed a request.");


            BastionModel.TenantID = "885f2aa5-368e-4665-a547-5b31239f92ad";
            BastionModel.SubscriptionID = "e9b63d88-2fb5-49f6-8d1d-2ef618a7f43c";
            BastionModel.ResourceGroup = "Testing";
            BastionModel.SPN = "14bcfc5a-4f24-4c00-8443-9be6b62a3ce3";
            BastionModel.SPNKEY = "PUcJjF_BM4xrr3xVY1~5C~UE3zC~xoiP-1";
            BastionModel.BastionName = "DemoRun";
            BastionModel.PublicIP = "AUBastion";
            BastionModel.VNet = "VNetAUEast";
            BastionModel.BastionSku = "Basic";
            BastionModel.Location = "australiaeast";


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

            Authorization.GetAuthorizationToken();

            //Bastion.getBastion(config).GetAwaiter().GetResult(); 
            Bastion.createBastion(log).GetAwaiter().GetResult();
            //Check if PublicIP and VNet are set
            // responseMessage = $"{BastionModel.Response} Tenant : {BastionModel.TenantID}. SUBSCRIPTION : {BastionModel.SubscriptionID}. SPN : {BastionModel.SPN}. SPNKEY : {BastionModel.SPNKEY}. BastionName : {BastionModel.BastionName}. ReourceGroup : {BastionModel.ResourceGroup}. PublicIP {BastionModel.PublicIP}. VNET : {BastionModel.VNet}";
            responseMessage = BastionModel.Response;

            return new OkObjectResult(responseMessage);

        }
    }
}
