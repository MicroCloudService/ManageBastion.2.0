using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

using System.Net;
using System.Net.Http;
using System.Text;

namespace ManageBastion2
{
    public static class GetBastionId
    {
        [FunctionName("GetBastionId")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestMessage req,
            ExecutionContext context,
            ILogger log)
        {
            var config = new ConfigurationBuilder()
                 .SetBasePath(context.FunctionAppDirectory)
                 // This gives you access to your application settings 
                // in your local development environment
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                // This is what actually gets you the 
                // application settings in Azure
                .AddEnvironmentVariables()
                .Build();

            string varSubscriptionId = config["SUBSCRIPTION"];
            string varBastionName = config["BASTION_NAME"];
            
            string varResourceGroup = config["RESOURCE_GROUP"];
            string varBastionId = $"https://management.azure.com/subscriptions/{varSubscriptionId}/resourceGroups/{varResourceGroup}/providers/Microsoft.Network/bastionHosts/{varBastionName}?api-version=2022-01-01";

            log.LogInformation("C# HTTP trigger function processed a request.");

            var myObj = new { bastionName = varBastionId };
            var jsonToReturn = JsonConvert.SerializeObject(myObj);

            //var content = new StringContent(body, Encoding.UTF8, "application/json");
            //request.Content = content;
            //var response = await Common.MakeRequestAsync(request, client);
            //BastionModel.Response = response;

            //return new HttpResponseMessage(HttpStatusCode.OK)
            //{
            //    Content = new StringContent(jsonToReturn, Encoding.UTF8, "application/json")
            //};

            var createResponse = req.CreateResponse(HttpStatusCode.OK);
            createResponse.Content = new StringContent(jsonToReturn, System.Text.Encoding.UTF8, "application/json");
            return createResponse;


           // return new OkObjectResult(responseMessage);
        }
    }
}
