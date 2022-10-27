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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
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

using Microsoft.Extensions.Configuration;


namespace ManageBastion2
{
    public static class GetBastionSettings
    {

        public static async Task GetSettings(ILogger log, ExecutionContext context)
        {
            try
            {
                log.LogInformation("The version of the currently executing assembly is: 1.2.0");


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

                if (!string.IsNullOrEmpty(config[Constants.PublicIP]) && !string.IsNullOrEmpty(config[Constants.VNetId]))
                {
                    // AzureServiceTokenProvider will help us to get the Service Managed token.
                    var azureServiceTokenProvider = new AzureServiceTokenProvider();

                    // Authenticate to the Azure Resource Manager to get the Service Managed token.
                    BastionModel.AccessToken = await azureServiceTokenProvider.GetAccessTokenAsync("https://management.azure.com/");



                    log.LogInformation("Get Bastion");
                    log.LogInformation("BastionModel.FunctionName :" + BastionModel.FunctionName);

                    log.LogInformation("BastionModel.BastionName :" + BastionModel.BastionName);
                    Bastion.getBastion(config, log).GetAwaiter().GetResult();

                }


                log.LogInformation("VNet config:" + config[Constants.VNet]);
                log.LogInformation("PubIP config:" + config[Constants.PublicIP]);
                log.LogInformation("VNet_Id config:" + config[Constants.VNetId]);
                log.LogInformation("PubIP_Id config:" + config[Constants.PublicIPId]);

                log.LogInformation("BastionModel.ManagedResourceGroup :" + BastionModel.ManagedResourceGroup);
                log.LogInformation("BastionModel.BastionName :" + BastionModel.BastionName);

            }
            catch (Exception ex)
            {
                log.LogInformation("Error :" + ex.Message.ToString());
                throw ex;
            }
        }


    }
}
