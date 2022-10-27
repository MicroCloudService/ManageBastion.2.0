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
using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;




namespace ManageBastion2
{


    public static class TurnOnBastion
    {
        [FunctionName("TurnOnBastion")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            string responseMessage = "";
            log.LogInformation("The version of the currently executing assembly is: 1.2.0");


            log.LogInformation("C# HTTP trigger function processed a request.");
            try
            {
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
                BastionModel.FunctionName= config[Constants.FunctionName];
                BastionModel.ManagedResourceGroup = config[Constants.ManagedResourceGroupName];


                BastionModel.PublicIP = config[Constants.PublicIP];
                BastionModel.PublicIPId = config[Constants.PublicIPId];
                BastionModel.VNet = config[Constants.VNet];
                BastionModel.VNetId = config[Constants.VNetId];

                BastionModel.Location = config[Constants.Location];

                //BastionModel.TenantID = req.Query[Constants.TenantID];
                //BastionModel.SubscriptionID = req.Query[Constants.SubscriptionID];
                //BastionModel.SPN = req.Query[Constants.SPN];
                //BastionModel.SPNKEY = req.Query[Constants.SPNKEY];
                //BastionModel.BastionName = req.Query[Constants.BastionName];
                //BastionModel.ResourceGroup = req.Query[Constants.ResourceGroup];
                //BastionModel.PublicIP = req.Query[Constants.PublicIP];
                //BastionModel.VNet = req.Query[Constants.VNet];

                //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                //dynamic data = JsonConvert.DeserializeObject(requestBody);

                //BastionModel.TenantID = BastionModel.TenantID ?? data.tenant;
                //BastionModel.SubscriptionID = BastionModel.SubscriptionID ?? data.subscription;
                //BastionModel.SPN = BastionModel.SPN ?? data.spn;
                //BastionModel.SPNKEY = BastionModel.SPNKEY ?? data.spnkey;
                //BastionModel.BastionName = BastionModel.BastionName ?? data.bastionName;
                //BastionModel.ResourceGroup = BastionModel.ResourceGroup ?? data.resourceGroup;
                //BastionModel.PublicIP = BastionModel.PublicIP ?? data.publicIP;
                //BastionModel.VNet = BastionModel.VNet ?? data.vnet;

                //BastionModel.TenantID = BastionModel.TenantID ?? SupportFunctions.GetEnvironmentVariable(Constants.TenantID);
                //BastionModel.SubscriptionID = BastionModel.SubscriptionID ?? SupportFunctions.GetEnvironmentVariable(Constants.SubscriptionID);
                //BastionModel.SPN = BastionModel.SPN ?? SupportFunctions.GetEnvironmentVariable(Constants.SPN);
                //BastionModel.SPNKEY = BastionModel.SPNKEY ?? SupportFunctions.GetEnvironmentVariable(Constants.SPNKEY);
                //BastionModel.BastionName = BastionModel.BastionName ?? SupportFunctions.GetEnvironmentVariable(Constants.BastionName);
                //BastionModel.ResourceGroup = BastionModel.ResourceGroup ?? SupportFunctions.GetEnvironmentVariable(Constants.ResourceGroup);
                //BastionModel.PublicIP = BastionModel.PublicIP ?? SupportFunctions.GetEnvironmentVariable(Constants.PublicIP);
                //BastionModel.VNet = BastionModel.VNet ?? SupportFunctions.GetEnvironmentVariable(Constants.VNet);

                //BastionModel.FunctionName =  SupportFunctions.GetEnvironmentVariable(Constants.FunctionName) ?? BastionModel.FunctionName ;
                //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                //dynamic data = JsonConvert.DeserializeObject(requestBody);
                //name = name ?? data?.name;

                //BastionModel.TenantID = "885f2aa5-368e-4665-a547-5b31239f92ad";
                //BastionModel.SubscriptionID = "e9b63d88-2fb5-49f6-8d1d-2ef618a7f43c";
                //BastionModel.ResourceGroup = "Testing";
                //BastionModel.SPN = "14bcfc5a-4f24-4c00-8443-9be6b62a3ce3";
                //BastionModel.SPNKEY = "PUcJjF_BM4xrr3xVY1~5C~UE3zC~xoiP-1";
                //BastionModel.BastionName = "DemoRun";
                //BastionModel.PublicIP = "AUBastion";
                //BastionModel.VNet = "VNetAUEast";
                //BastionModel.FunctionName = "ManageBastion";
                //Authorization.GetAuthorizationToken();



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

                    log.LogInformation("BastionModel.BastionName :" + BastionModel.BastionName);
                    log.LogInformation("BastionModel.Location :" + BastionModel.Location);
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

                    log.LogInformation("BastionModel.Exists :" + BastionModel.Exists);
                    log.LogInformation("BastionModel.ProvisioningState :" + BastionModel.ProvisioningState);

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

                    log.LogInformation("BastionModel.ManagedResourceGroup :" + BastionModel.ManagedResourceGroup);
                    log.LogInformation("BastionModel.BastionName :" + BastionModel.BastionName);
                    responseMessage = "Bastion Already On!";
                }
                return new OkObjectResult(responseMessage);
            }
            catch (Exception ex)
            {
                responseMessage = ex.Message;
                return new OkObjectResult(responseMessage);
                // throw;
            }






        }














    }
}
