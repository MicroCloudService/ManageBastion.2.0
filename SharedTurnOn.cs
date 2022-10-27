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
    public static class SharedTurnOn
    {
        public static async Task<IActionResult> Run(
           ILogger log, ExecutionContext context)
        {

            string responseMessage = "";

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
            BastionModel.VNet = config[Constants.VNet];
            BastionModel.VNetId = config[Constants.VNetId];
            BastionModel.Location = config[Constants.Location];



            // AzureServiceTokenProvider will help us to get the Service Managed token.
            var azureServiceTokenProvider = new AzureServiceTokenProvider();

            // Authenticate to the Azure Resource Manager to get the Service Managed token.
            BastionModel.AccessToken = await azureServiceTokenProvider.GetAccessTokenAsync("https://management.azure.com/");



            //Bastion.getApplicationSettings()



            //Bastion.getBastion(config).GetAwaiter().GetResult(); 
            //Bastion.createBastion(log).GetAwaiter().GetResult();
            //Check if PublicIP and VNet are set
            if (!string.IsNullOrEmpty(config[Constants.PublicIP]) && !string.IsNullOrEmpty(config[Constants.VNetId]))
            {
                //config[Constants.VNet] = BastionModel.VNet;
                //string value = config[Constants.VNet];
                //config[Constants.VNet] = BastionModel.PublicIP;
                //string pubipvalue = config[Constants.PublicIP];

                log.LogInformation("VNet config:" + config[Constants.VNet]);
                log.LogInformation("PubIP config:" + config[Constants.PublicIP]);
                log.LogInformation("VNet_Id config:" + config[Constants.VNetId]);
                log.LogInformation("PubIP_Id config:" + config[Constants.PublicIPId]);
                log.LogInformation("Location config:" + config[Constants.Location]);
                log.LogInformation("BastionModel.FunctionName :" + BastionModel.FunctionName);
                log.LogInformation("BastionModel.ManagedResourceGroup :" + BastionModel.ManagedResourceGroup);
                log.LogInformation("BastionModel.PublicIPId :" + BastionModel.PublicIPId);
                log.LogInformation("BastionModel.VNetId :" + BastionModel.VNetId);
                log.LogInformation("BastionModel.Location :" + BastionModel.Location);

                log.LogInformation("BastionModel.BastionName :" + BastionModel.BastionName);
                //if not set then set
                //Get Public IP

                log.LogInformation("Create Bastion");


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

                if (!BastionModel.Exists && (BastionModel.ProvisioningState != "Updating" || BastionModel.ProvisioningState != "Deleting"))
                {
                    Bastion.createBastion(log).GetAwaiter().GetResult();
                    responseMessage = "Bastion Turning On!";
                }
                else
                {
                    responseMessage = "Bastion Already On!";
                }
                // responseMessage = BastionModel.Response;
                responseMessage = "Bastion Already On!";
                //SupportFunctions.SetEnvironmentVariable(Constants.PublicIP,)
                //    SupportFunctions.SetEnvironmentVariable
                //responseMessage = $"{BastionModel.Response} Tenant : {BastionModel.TenantID}. SUBSCRIPTION : {BastionModel.SubscriptionID}. SPN : {BastionModel.SPN}. SPNKEY : {BastionModel.SPNKEY}. BastionName : {BastionModel.BastionName}. ReourceGroup : {BastionModel.ResourceGroup}. PublicIP {BastionModel.PublicIP}. VNET : {BastionModel.VNet}";
            }
            else
            {
                log.LogInformation("Get Bastion");
                log.LogInformation("BastionModel.FunctionName :" + BastionModel.FunctionName);

                log.LogInformation("BastionModel.BastionName :" + BastionModel.BastionName);
                Bastion.getBastion(config, log).GetAwaiter().GetResult();


                if (SupportFunctions.EnvironmentVariableExists(config[Constants.VNetId]))
                {
                    //config[Constants.VNet] = BastionModel.VNet;
                    //config[Constants.VNetId] = BastionModel.VNetId;
                }
                //string value = config[Constants.VNet];
                if (SupportFunctions.EnvironmentVariableExists(config[Constants.PublicIP]))
                {
                    //config[Constants.PublicIP] = BastionModel.PublicIP;
                    //config[Constants.PublicIPId] = BastionModel.PublicIPId;
                }

                //string pubipvalue = config[Constants.PublicIP];

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

                responseMessage = "Bastion Already On!";
            }
            return new OkObjectResult(responseMessage);
        }


    }
}
