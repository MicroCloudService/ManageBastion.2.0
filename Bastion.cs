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
    static class Bastion
    {
        public static async Task getBastion(IConfigurationRoot config, ILogger log)
        {
            BastionModel.Exists = false;
            HttpClient client = new HttpClient();
            //PUT https://management.azure.com/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Network/bastionHosts/{bastionHostName}?api-version=2021-03-01
            client.BaseAddress = new Uri("https://management.azure.com/subscriptions/" + BastionModel.SubscriptionID + "/resourcegroups/" + BastionModel.ResourceGroup + "/providers/Microsoft.Network/bastionHosts/" + BastionModel.BastionName + "?api-version=2021-03-01");
            // GET https://management.azure.com/subscriptions/subid/resourceGroups/rg1/providers/Microsoft.Network/bastionHosts/bastionhosttenant'?api-version=2021-03-01

            log.LogInformation("Get Bastion Base Address :" + client.BaseAddress);
            client.DefaultRequestHeaders.Accept.Clear();

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + BastionModel.AccessToken);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, client.BaseAddress);

            string json="";

  json = await Common.MakeRequestAsync(request, client);
            log.LogInformation("GetBastion Return Value :" + json);

            log.LogInformation("GetBastion Status :" + BastionModel.Status);

            if (BastionModel.Status.ToLower().ToString() == HttpStatusCode.OK.ToString().ToLower())
            {  
               
                BastionModel.Exists = true;
                JObject data = JObject.Parse(json);

                //JObject data = JsonConvert.DeserializeObject<JObject>(json);
                //data = (JObject)data.ToString().Remove(0, 1);
                // data = (JObject)data.ToString().Remove(data.ToString().Length - 1, 1);
                GetVNetSaveAppSetting(data, config,log);
                GetPublicIPSaveAppSetting(data, config, log);
                GetSkuSaveAppSetting(data, config, log);
                GetLocationSaveAppSetting(data, config, log);
                GetProvisioningState(data, config, log);
                //GetOffSchedule(data, config, log);
               // GetOnSchedule(data, config, log);

                // Part 2: loop over list and print pairs.
                //foreach (var element in list)
                //{
                //    Console.WriteLine(element);
                //}

                log.LogInformation("Before getApplicationSettings");

               await AddAppSettings.getApplicationSettings( log);
                log.LogInformation("After getApplicationSettings");

                log.LogInformation("Before BuildAppSettings");
                await AddAppSettings .BuildAppSettings(log);
                log.LogInformation("After BuildAppSettings");

                log.LogInformation("Before AddApplicationSettings");
               await  AddAppSettings.AddApplicationSettings(log);
                log.LogInformation("After AddApplicationSettings");
            }
            // dynamic results = JsonConvert.DeserializeObject<dynamic>(json);





            BastionModel.Response = json;
        }


        public static async Task deleteBastion(ILogger log)
        {
            HttpClient client = new HttpClient();
            //PUT https://management.azure.com/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Network/bastionHosts/{bastionHostName}?api-version=2021-03-01
            client.BaseAddress = new Uri("https://management.azure.com/subscriptions/" + BastionModel.SubscriptionID + "/resourcegroups/" + BastionModel.ResourceGroup + "/providers/Microsoft.Network/bastionHosts/" + BastionModel.BastionName + "?api-version=2021-03-01");
            // GET https://management.azure.com/subscriptions/subid/resourceGroups/rg1/providers/Microsoft.Network/bastionHosts/bastionhosttenant'?api-version=2021-03-01
            //DELETE https://management.azure.com/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Network/bastionHosts/{bastionHostName}?api-version=2021-03-01

            log.LogInformation("Get Bastion Base Address :" + client.BaseAddress);
            client.DefaultRequestHeaders.Accept.Clear();

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + BastionModel.AccessToken);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, client.BaseAddress);

            var json = await Common.MakeRequestAsync(request, client);
            // dynamic results = JsonConvert.DeserializeObject<dynamic>(json);

            JObject data = JsonConvert.DeserializeObject<JObject>(json);
            //data = (JObject)data.ToString().Remove(0, 1);
            // data = (JObject)data.ToString().Remove(data.ToString().Length - 1, 1);
            //GetVNetSaveAppSetting(data, log);
            // GetPublicIPSaveAppSetting(data, log);
            if (BastionModel.Status == HttpStatusCode.Accepted.ToString())
            {
                BastionModel.Exists = false;
            }
            BastionModel.Response = json;
            log.LogInformation("Delete Bastion BastionModel.Response :" + BastionModel.Response);
        }

        private static void GetVNetSaveAppSetting(JObject data, IConfigurationRoot config, ILogger log)
        {
           // JObject properties = (JObject)data["properties"];
            //JArray ipConfigurations = (JArray)properties["ipConfigurations"];
            //JObject properties2 = (JObject)ipConfigurations["properties"];
            //JObject properties2 = (JObject)properties2["publicIPAddress"]

            JToken subnetId = data.SelectToken("$..subnet.id");
            Regex regex = new Regex(@"[^/]+(?=/$|$)");
            log.LogInformation("pre  vnetvalue: " + subnetId.ToString());
            var vnetvalue = subnetId.ToString().Substring(0,subnetId.ToString().IndexOf("/subnets/"));
            log.LogInformation("VNetValue  vnetvalue: " + vnetvalue);
            Match subnetValue = regex.Match(vnetvalue);
            Console.WriteLine(subnetId);
            Console.WriteLine(subnetValue);
            log.LogInformation("VNetId:" + subnetId);
             log.LogInformation("VNetValue subnetValue: " + subnetValue);
            BastionModel.VNet = subnetValue.ToString();
            BastionModel.VNetId = subnetId.ToString();
            //SupportFunctions.SetEnvironmentVariable(config, Constants.VNet, BastionModel.VNet);
            //SupportFunctions.SetEnvironmentVariable(config, Constants.VNetId, BastionModel.VNetId);
        }

        private static void GetPublicIPSaveAppSetting(JObject data, IConfigurationRoot config, ILogger log)
        {
            JToken publicIPId = data.SelectToken("$..publicIPAddress.id");
            Regex regex = new Regex(@"[^/]+(?=/$|$)");
            Match publicIPValue = regex.Match(publicIPId.ToString());
            // Console.WriteLine(publicIPId);
            // Console.WriteLine(publicIPValue);
             log.LogInformation("Public_IP Id:" + publicIPId);
             log.LogInformation("Public_IP : " + publicIPValue);
            BastionModel.PublicIP = publicIPValue.ToString();
            BastionModel.PublicIPId = publicIPId.ToString();

            //SupportFunctions.SetEnvironmentVariable(config, Constants.PublicIP, BastionModel.PublicIP);
           // SupportFunctions.SetEnvironmentVariable(config, Constants.PublicIPId, BastionModel.PublicIPId);
        }

        private static void GetSkuSaveAppSetting(JObject data, IConfigurationRoot config, ILogger log)
        {
            JToken bastionsku = data.SelectToken("$..sku.name");
            BastionModel.BastionSku = bastionsku.ToString();
            log.LogInformation("BastionSku:" + bastionsku);
            //SupportFunctions.SetEnvironmentVariable(config, Constants.SKU, BastionModel.BastionSku);
        }

        private static void GetLocationSaveAppSetting(JObject data, IConfigurationRoot config, ILogger log)
        {
            JToken bastionlocation = data.SelectToken("$..location");
            BastionModel.Location = bastionlocation.ToString();
            log.LogInformation("bastionlocation:" + bastionlocation);
           // SupportFunctions.SetEnvironmentVariable(config, Constants.Location, BastionModel.Location);
        }

        private static void GetProvisioningState(JToken vnet, IConfigurationRoot config, ILogger log)
        {
            JToken provisioningState = vnet.SelectToken("$.properties.provisioningState");

            log.LogInformation("provisioningState:" + provisioningState);
            BastionModel.ProvisioningState = provisioningState.ToString();
            //SupportFunctions.SetEnvironmentVariable(config, Constants., BastionModel.BastionSku);
        }

        private static void GetOffSchedule(JToken vnet, IConfigurationRoot config, ILogger log)
        {
            JToken offTimerSchedule = vnet.SelectToken("$.properties.OffTimerSchedule");

            log.LogInformation("provisioningState:" + offTimerSchedule);
            BastionModel.OffTimerSchedule = offTimerSchedule.ToString();
            //SupportFunctions.SetEnvironmentVariable(config, Constants., BastionModel.BastionSku);
        }

        private static void GetOnSchedule(JToken vnet, IConfigurationRoot config, ILogger log)
        {
            JToken onTimerSchedule = vnet.SelectToken("$.properties.OnTimerSchedule");

            log.LogInformation("OnTimerSchedule:" + onTimerSchedule);
            BastionModel.OnTimerSchedule = onTimerSchedule.ToString();
            //SupportFunctions.SetEnvironmentVariable(config, Constants., BastionModel.BastionSku);
        }

        public static async Task createBastion(ILogger log)
        {
            HttpClient client = new HttpClient();
            //PUT https://management.azure.com/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Network/bastionHosts/{bastionHostName}?api-version=2021-03-01
            client.BaseAddress = new Uri("https://management.azure.com/subscriptions/" + BastionModel.SubscriptionID + "/resourcegroups/" + BastionModel.ResourceGroup + "/providers/Microsoft.Network/bastionHosts/" + BastionModel.BastionName + "?api-version=2021-03-01");
            log.LogInformation("Address : " + client.BaseAddress);
            //client.BaseAddress = new Uri("https://management.azure.com/subscriptions/" + BastionModel.SubscriptionID + "/resourcegroups/" + BastionModel.ResourceGroup + "?api-version=2019-10-01");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + BastionModel.AccessToken);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, client.BaseAddress);
            //BastionModel.VNetId = "/ subscriptions / subid / resourceGroups / rg1 / providers / Microsoft.Network / virtualNetworks / vnet2 / subnets / BastionHostSubnet"";
            //BastionModel.PublicIPId = "";
            //BastionModel.BastionSku = "Basic";
           // BastionModel.Location = "australiaeast";

            var body = @"{
  " + "\n" +
  @$"  ""location"": ""{BastionModel.Location}""
" + "\n" +
  @" ""sku"": {
  " + "\n" +
  @$"  ""name"": ""{BastionModel.BastionSku}""
" + "\n" +
  @" ""},
" + "\n" +
 @" ""properties"": {
  " + "\n" +
 @"                 ""ipConfigurations"": [
  " + "\n" +
@" ""{
                " + "\n" +
                    @" ""name"": ""bastionHostIpConfiguration"",
  " + "\n" +
        @" ""properties"": {
                    " + "\n" +
                        @"""subnet"": {
                        " + "\n" +
                            @$" ""id"": ""/subscriptions/{BastionModel.SubscriptionID}/resourceGroups/{BastionModel.ResourceGroup}/providers/Microsoft.Network/virtualNetworks/{BastionModel.VNet}/subnets/AzureBastionSubnet""
  " + "\n" +
                        @" ""},
  " + "\n" +
          @" ""publicIPAddress"": {
                        " + "\n" +
                            @$" ""id"": ""/subscriptions/{BastionModel.SubscriptionID}/resourceGroups/{BastionModel.ResourceGroup}/providers/Microsoft.Network/publicIPAddresses/{BastionModel.PublicIP}""
  " + "\n" +
          @" ""}
                    " + "\n" +
                    @" ""}
                " + "\n" +
                @" ""}
            " + "\n" +
    @" ""]
  " + "\n" +
  @" ""}
  " + "\n" +
       @" "" }";

            body = "";
            body = "{\"location\": \"" + BastionModel.Location + "\",\"sku\": {\"name\"" + BastionModel.BastionSku + "\"},\"properties\": {\"ipConfigurations\": [{\"name\": \"bastionHostIpConfiguration\",\"properties\": {\"subnet\": {\"id\": \"" + BastionModel.VNetId + "/\"},\"publicIPAddress\": {\"id\": \""  + BastionModel.PublicIPId + "/\"}}}]}}";

            log.LogInformation("Body Call : " + body);

            //            var body = $"{{\"location\": \"{BastionModel.Location}\"}}";
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            request.Content = content;
            var response = await Common.MakeRequestAsync(request, client);
            log.LogInformation("CreateBastion Return Value :" + response);


            BastionModel.Response = response;

        }

        public static async Task removeBastion()
        {
            HttpClient client = new HttpClient();
            //PUT https://management.azure.com/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Network/bastionHosts/{bastionHostName}?api-version=2021-03-01
            client.BaseAddress = new Uri("https://management.azure.com/subscriptions/" + BastionModel.SubscriptionID + "/resourcegroups/" + BastionModel.ResourceGroup + "providers/Microsoft.Network/bastionHosts/" + BastionModel.BastionName + "?api-version=2021-03-01?");

            //client.BaseAddress = new Uri("https://management.azure.com/subscriptions/" + BastionModel.SubscriptionID + "/resourcegroups/" + BastionModel.ResourceGroup + "?api-version=2019-10-01");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + BastionModel.AccessToken);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, client.BaseAddress);
            var body = @"{
  " + "\n" +
 @" ""properties"": {
  " + "\n" +
 @"                 ""ipConfigurations"": [
  " + "\n" +
@" ""{
                " + "\n" +
                    @" ""name"": ""bastionHostIpConfiguration"",
  " + "\n" +
        @" ""properties"": {
                    " + "\n" +
                        @"""subnet"": {
                        " + "\n" +
                            @$" ""id"": ""/subscriptions/{BastionModel.SubscriptionID}/resourceGroups/{BastionModel.ResourceGroup}/providers/Microsoft.Network/virtualNetworks/{BastionModel.VNet}/subnets/AzureBastionSubnet""
  " + "\n" +
                        @" ""},
  " + "\n" +
          @" ""publicIPAddress"": {
                        " + "\n" +
                            @$" ""id"": ""/subscriptions/{BastionModel.SubscriptionID}/resourceGroups/{BastionModel.ResourceGroup}/providers/Microsoft.Network/publicIPAddresses/{BastionModel.PublicIP}""
  " + "\n" +
          @" ""}
                    " + "\n" +
                    @" ""}
                " + "\n" +
                @" ""}
            " + "\n" +
    @" ""]
  " + "\n" +
  @" ""}
  " + "\n" +
       @" "" }";


            //            var body = $"{{\"location\": \"{BastionModel.Location}\"}}";
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            request.Content = content;
            var response = await Common.MakeRequestAsync(request, client);
            BastionModel.Response = response;
        }
    }
}
