using System;
using System.Collections.Generic;
using System.Text;
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

using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace ManageBastion2
{
   public static class SharedTurnOff
    {
        public static async Task<IActionResult> Run(
         ILogger log, ExecutionContext context)
        {
            string responseMessage = "";
            log.LogInformation("C# HTTP trigger function processed a request.");
            //read using Microsoft.Extensions.Configuration

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

            BastionModel.TenantID = config[Constants.TenantID];
            BastionModel.SubscriptionID = config[Constants.SubscriptionID];
            BastionModel.SPN = config[Constants.SPN];
            BastionModel.SPNKEY = config[Constants.SPNKEY];
            BastionModel.BastionName = config[Constants.BastionName];
            BastionModel.ResourceGroup = config[Constants.ResourceGroup];
            BastionModel.FunctionName = config[Constants.FunctionName];
            BastionModel.ManagedResourceGroup = config[Constants.ManagedResourceGroupName];
            BastionModel.PublicIP = config[Constants.PublicIP];
            BastionModel.PublicIPId = config[Constants.PublicIPId];
            BastionModel.VNet=config[Constants.VNet];
            BastionModel.VNetId = config[Constants.VNetId];
            BastionModel.Location = config[Constants.Location];




            var azureServiceTokenProvider = new AzureServiceTokenProvider();

            // Authenticate to the Azure Resource Manager to get the Service Managed token.
            BastionModel.AccessToken = await azureServiceTokenProvider.GetAccessTokenAsync("https://management.azure.com/");



            //Authorization.GetAuthorizationToken();

            //Check if PublicIP and VNet are set
            if (!string.IsNullOrEmpty(config[Constants.PublicIP]) && !string.IsNullOrEmpty(config[Constants.VNetId]))
            {
                log.LogInformation("Config Vars Exist");
                log.LogInformation("VNet config:" + config[Constants.VNet]);
                log.LogInformation("PubIP config:" + config[Constants.PublicIP]);
                log.LogInformation("VNet_Id config:" + config[Constants.VNetId]);
                log.LogInformation("PubIP_Id config:" + config[Constants.PublicIPId]);
                log.LogInformation("Location config:" + config[Constants.Location]);
                log.LogInformation("BastionModel.FunctionName :" + BastionModel.FunctionName);
                log.LogInformation("BastionModel.ManagedResourceGroup :" + BastionModel.ManagedResourceGroup);
                log.LogInformation("BastionModel.PublicIPId :" + BastionModel.PublicIPId);
                log.LogInformation("BastionModel.VNetId :" + BastionModel.VNetId);
                log.LogInformation("BastionModel.BastionName :" + BastionModel.BastionName);
                log.LogInformation("BastionModel.Location :" + BastionModel.Location);
                //if not set then set
                //Get Public IP
                Bastion.getBastion(config, log).GetAwaiter().GetResult();

                log.LogInformation("VNet config:" + config[Constants.VNet]);
                log.LogInformation("PubIP config:" + config[Constants.PublicIP]);
                log.LogInformation("VNet_Id config:" + config[Constants.VNetId]);
                log.LogInformation("PubIP_Id config:" + config[Constants.PublicIPId]);
                log.LogInformation("Location config:" + config[Constants.Location]);
                log.LogInformation("BastionModel.FunctionName :" + BastionModel.FunctionName);
                log.LogInformation("BastionModel.ManagedResourceGroup :" + BastionModel.ManagedResourceGroup);
                log.LogInformation("BastionModel.PublicIPId :" + BastionModel.PublicIPId);
                log.LogInformation("BastionModel.VNetId :" + BastionModel.VNetId);
                log.LogInformation("BastionModel.BastionName :" + BastionModel.BastionName);
                log.LogInformation("BastionModel.Location :" + BastionModel.Location);

                if (BastionModel.Exists && (BastionModel.ProvisioningState == "Succeeded"))
                {
                    Bastion.deleteBastion(log).GetAwaiter().GetResult();
                    responseMessage = "Bastion Turning Off!";
                }
                else
                {
                    responseMessage = "Bastion Already Off!";
                }
                // SupportFunctions.SetEnvironmentVariable(config,Constants.PublicIP, BastionModel.PublicIP);
                //    log.LogInformation("VNetId:" + BastionModel.VNetId);
                //log.LogInformation("VNetValue : " + BastionModel.VNet);

                ////log.LogInformation("VNetValue AppSettings : " + SupportFunctions.SetEnvironmentVariable(Constants.VNet, BastionModel.VNet));

                //log.LogInformation("Public_IP Id:" + BastionModel.PublicIPId);
                //log.LogInformation("Public_IP : " + BastionModel.PublicIP);

                // log.LogInformation("Public_IPValue AppSettings : " + SupportFunctions.SetEnvironmentVariable(config, Constants.PublicIP, BastionModel.PublicIP));


                //SupportFunctions.SetEnvironmentVariable(Constants.PublicIP,)
                //    SupportFunctions.SetEnvironmentVariable
                // responseMessage = $"Tenant : {BastionModel.TenantID}. SUBSCRIPTION : {BastionModel.SubscriptionID}. SPN : {BastionModel.SPN}. SPNKEY : {BastionModel.SPNKEY}. BastionName : {BastionModel.BastionName}. ReourceGroup : {BastionModel.ResourceGroup}. PublicIP {BastionModel.PublicIP}. VNET : {BastionModel.VNet}";
            }
            else
            {
                log.LogInformation("Config Vars Don't Exist");
                log.LogInformation("VNet config:" + config[Constants.VNet]);
                log.LogInformation("PubIP config:" + config[Constants.PublicIP]);
                log.LogInformation("VNet_Id config:" + config[Constants.VNetId]);
                log.LogInformation("PubIP_Id config:" + config[Constants.PublicIPId]);
                log.LogInformation("Location config:" + config[Constants.Location]);
                log.LogInformation("BastionModel.FunctionName :" + BastionModel.FunctionName);
                log.LogInformation("BastionModel.ManagedResourceGroup :" + BastionModel.ManagedResourceGroup);
                log.LogInformation("BastionModel.PublicIPId :" + BastionModel.PublicIPId);
                log.LogInformation("BastionModel.VNetId :" + BastionModel.VNetId);
                log.LogInformation("BastionModel.BastionName :" + BastionModel.BastionName);
                log.LogInformation("BastionModel.Location :" + BastionModel.Location);

                Bastion.getBastion(config, log).GetAwaiter().GetResult();

                log.LogInformation("VNet config:" + config[Constants.VNet]);
                log.LogInformation("PubIP config:" + config[Constants.PublicIP]);
                log.LogInformation("VNet_Id config:" + config[Constants.VNetId]);
                log.LogInformation("PubIP_Id config:" + config[Constants.PublicIPId]);
                log.LogInformation("Location config:" + config[Constants.Location]);
                log.LogInformation("BastionModel.FunctionName :" + BastionModel.FunctionName);
                log.LogInformation("BastionModel.ManagedResourceGroup :" + BastionModel.ManagedResourceGroup);
                log.LogInformation("BastionModel.PublicIPId :" + BastionModel.PublicIPId);
                log.LogInformation("BastionModel.VNetId :" + BastionModel.VNetId);
                log.LogInformation("BastionModel.BastionName :" + BastionModel.BastionName);
                log.LogInformation("BastionModel.Location :" + BastionModel.Location);

                if (BastionModel.Exists && (BastionModel.ProvisioningState == "Succeeded"))
                {
                    Bastion.deleteBastion(log).GetAwaiter().GetResult();
                    responseMessage = "Bastion Turning Off!";
                }
                else
                {
                    responseMessage = "Bastion Already Off!";
                }

               
            }
            return new OkObjectResult(responseMessage);
        }
        }
}
