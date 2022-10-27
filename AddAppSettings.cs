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

using Microsoft.Extensions.Configuration;


namespace ManageBastion2
{
    public static class AddAppSettings
    {

        public static async Task AddApplicationSettings(ILogger log)
        {
            try
            {
                var body = BastionModel.NewAppSettings;// settings;
                                                       //"{\"location\": \"" + BastionModel.Location + "\", \"properties\": {\"ipConfigurations\": [{\"name\": \"bastionHostIpConfiguration\",\"properties\": {\"subnet\": {\"id\": \"/subscriptions/" + BastionModel.SubscriptionID + "/resourceGroups/" + BastionModel.ResourceGroup + "/providers/Microsoft.Network/virtualNetworks/" + BastionModel.VNet + "/subnets/AzureBastionSubnet\"},\"publicIPAddress\": {\"id\": \"/subscriptions/" + BastionModel.SubscriptionID + "/resourceGroups/" + BastionModel.ResourceGroup + "/providers/Microsoft.Network/publicIPAddresses/" + BastionModel.PublicIP + "\"}}}]}}";
                log.LogInformation("New AppSettings Body :" + body);

                HttpClient client = new HttpClient();
                // @"subscriptions/xxxxxxxxxxxxxxxxxxx/resourceGroups/xxxxxx/providers/Microsoft.Web/sites/xxxxxx/config/appsettings?api-version=2016-08-01";


                client.BaseAddress = new Uri("https://management.azure.com/subscriptions/" + BastionModel.SubscriptionID + "/resourcegroups/" + BastionModel.ManagedResourceGroup + "/providers/Microsoft.Web/sites/" + BastionModel.FunctionName + "/config/appsettings?api-version=2021-02-01");

                log.LogInformation("AddApplicationSettings baseaddress :" + client.BaseAddress);

                client.DefaultRequestHeaders.Accept.Clear();

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + BastionModel.AccessToken);

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, client.BaseAddress);

                var content = new StringContent(body, Encoding.UTF8, "application/json");
                request.Content = content;
                var response = await Common.MakeRequestAsync(request, client);
                log.LogInformation("AddApplicationSettings Status :" + BastionModel.Status);
                log.LogInformation("AddApplicationSettings Return Value :" + response);

            }
            catch (Exception ex)
            {
                log.LogInformation("Error :" + ex.Message.ToString());
                throw ex;
            }
        }


        public static async Task getApplicationSettings(ILogger log)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://management.azure.com/subscriptions/" + BastionModel.SubscriptionID + "/resourcegroups/" + BastionModel.ManagedResourceGroup + "/providers/Microsoft.Web/sites/" + BastionModel.FunctionName + "/config/appsettings/list?api-version=2021-02-01");
                client.DefaultRequestHeaders.Accept.Clear();
                log.LogInformation("getApplicationSettings baseaddress :" + client.BaseAddress);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + BastionModel.AccessToken);

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress);

                //var response = await Common.MakeRequestAsync(request, client);
                var json = await Common.MakeRequestAsync(request, client);

                log.LogInformation("getApplicationSettings Status :" + BastionModel.Status);
                log.LogInformation("getApplicationSettings Return Value :" + json);

               // JObject appSettings = JObject.Parse(json);
                BastionModel.OldSettings = json;
                // log.LogInformation("OldSettings : " + BastionModel.OldSettings.ToString());
                // JObject properties = (JObject)appSettings["properties"];
                // var testssd = new JProperty("cat", "123");
                // properties.AddFirst(testssd);

                //JArray properties = (JArray)rss["properties"];
                //JToken newsetting = new JToken("sdsds", "sdsadad");
                //var test = new KeyValuePair<string, string>("Cat", "1");
                // properties.Add(JsonConvert.SerializeObject(test));
                //BastionModel.NewAppSettings = properties.ToString();
                //JObject data = JsonConvert.DeserializeObject<JObject>(json);

                BastionModel.Response = json;
            }
            catch (Exception ex)
            {
                log.LogInformation("Error :" + ex.Message.ToString());
                throw ex;
            }
        }

        public static async Task getApplicationSettings1()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://management.azure.com/subscriptions/" + BastionModel.SubscriptionID + "/resourcegroups/" + BastionModel.ManagedResourceGroup + "/providers/Microsoft.Web/sites/" + BastionModel.FunctionName + "/config/appsettings/list?api-version=2021-02-01");
            client.DefaultRequestHeaders.Accept.Clear();

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + BastionModel.AccessToken);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress);

            //var response = await Common.MakeRequestAsync(request, client);
            var json = await Common.MakeRequestAsync(request, client);
            JObject appSettings = JObject.Parse(json);
            BastionModel.OldSettings = json;// appSettings;


            //log.LogInformation("OldSettings : "+ BastionModel.OldSettings.ToString());
            // JObject properties = (JObject)appSettings["properties"];
            //var testssd = new JProperty("cat", "123");
            //properties.AddFirst(testssd);

            //JArray properties = (JArray)rss["properties"];
            //JToken newsetting = new JToken("sdsds", "sdsadad");
            //var test = new KeyValuePair<string, string>("Cat", "1");
            // properties.Add(JsonConvert.SerializeObject(test));
            //BastionModel.NewAppSettings = rss.ToString();
            //JObject data = JsonConvert.DeserializeObject<JObject>(json);

            BastionModel.Response = json;

        }

        public static async Task BuildAppSettings(ILogger log)
        {
            try
            {
                //var list = new List<KeyValuePair<string, string>>();
                //list.Add(new KeyValuePair<string, string>(Constants.VNet, BastionModel.VNet));
                //list.Add(new KeyValuePair<string, string>(Constants.VNetId, BastionModel.VNetId));
                //list.Add(new KeyValuePair<string, string>(Constants.PublicIP, BastionModel.PublicIP));
                //list.Add(new KeyValuePair<string, string>(Constants.PublicIPId, BastionModel.PublicIPId));
                //list.Add(new KeyValuePair<string, string>(Constants.SKU, BastionModel.BastionSku));
                //list.Add(new KeyValuePair<string, string>(Constants.Location, BastionModel.Location));
                JObject appSettings = JObject.Parse(BastionModel.OldSettings);
            JObject properties = (JObject)appSettings["properties"];

            //foreach (var setting in list)
            //{
            //var testssd = new JProperty(Constants.VNetId, BastionModel.VNetId);
            // ;
            // properties.Add(JsonConvert.SerializeObject(new KeyValuePair<string, string>("Cat", "1")));

            if (properties[Constants.VNet] == null) properties.AddFirst(new JProperty(Constants.VNet, BastionModel.VNet));

            if (properties[Constants.VNetId] == null) properties.AddFirst(new JProperty(Constants.VNetId, BastionModel.VNetId));
            if (properties[Constants.PublicIP] == null) properties.AddFirst(new JProperty(Constants.PublicIP, BastionModel.PublicIP));
            if (properties[Constants.PublicIPId] == null) properties.AddFirst(new JProperty(Constants.PublicIPId, BastionModel.PublicIPId));
            if (properties[Constants.SKU] == null) properties.AddFirst(new JProperty(Constants.SKU, BastionModel.BastionSku));
            if (properties[Constants.Location] == null) properties.AddFirst(new JProperty(Constants.Location, BastionModel.Location));

                //BastionModel.OldSettings["properties"] = properties.ToString();
                //appSettings["properties"] = properties;

                BastionModel.NewAppSettings = appSettings.ToString();
                // log.LogInformation("NewSettings : " + BastionModel.NewAppSettings);
                //}
            }
            catch (Exception ex)
            {

                log.LogInformation("Error :" + ex.Message.ToString());
                throw ex;
            }
        }

        public static async Task BuildAppSettingsWithSchedules(ILogger log)
        {
            try
            {
                //var list = new List<KeyValuePair<string, string>>();
                //list.Add(new KeyValuePair<string, string>(Constants.VNet, BastionModel.VNet));
                //list.Add(new KeyValuePair<string, string>(Constants.VNetId, BastionModel.VNetId));
                //list.Add(new KeyValuePair<string, string>(Constants.PublicIP, BastionModel.PublicIP));
                //list.Add(new KeyValuePair<string, string>(Constants.PublicIPId, BastionModel.PublicIPId));
                //list.Add(new KeyValuePair<string, string>(Constants.SKU, BastionModel.BastionSku));
                //list.Add(new KeyValuePair<string, string>(Constants.Location, BastionModel.Location));
                JObject appSettings = JObject.Parse(BastionModel.OldSettings);
                JObject properties = (JObject)appSettings["properties"];

                //foreach (var setting in list)
                //{
                //var testssd = new JProperty(Constants.VNetId, BastionModel.VNetId);
                // ;
                // properties.Add(JsonConvert.SerializeObject(new KeyValuePair<string, string>("Cat", "1")));

                if (properties[Constants.VNet] == null) properties.AddFirst(new JProperty(Constants.VNet, BastionModel.VNet));

                if (properties[Constants.VNetId] == null) properties.AddFirst(new JProperty(Constants.VNetId, BastionModel.VNetId));
                if (properties[Constants.PublicIP] == null) properties.AddFirst(new JProperty(Constants.PublicIP, BastionModel.PublicIP));
                if (properties[Constants.PublicIPId] == null) properties.AddFirst(new JProperty(Constants.PublicIPId, BastionModel.PublicIPId));
                if (properties[Constants.SKU] == null) properties.AddFirst(new JProperty(Constants.SKU, BastionModel.BastionSku));
                if (properties[Constants.Location] == null) properties.AddFirst(new JProperty(Constants.Location, BastionModel.Location));

                if (!String.IsNullOrEmpty(BastionModel.OffTimerSchedule))
                {
                    properties[Constants.OffTimerSchedule] = BastionModel.OffTimerSchedule;
                }

                if (!String.IsNullOrEmpty(BastionModel.OnTimerSchedule))
                {
                    properties[Constants.OnTimerSchedule] = BastionModel.OnTimerSchedule;
                }

                if (!String.IsNullOrEmpty(BastionModel.OffTimerScheduleDisabled))
                {
                    properties[Constants.OffTimerScheduleDisabled] = BastionModel.OffTimerScheduleDisabled;
                }

                if (!String.IsNullOrEmpty(BastionModel.OnTimerScheduleDisabled))
                {
                    properties[Constants.OnTimerScheduleDisabled] = BastionModel.OnTimerScheduleDisabled;
                }

                //appSettings["properties"] = properties;

                //BastionModel.OldSettings["properties"] = properties.ToString();
                BastionModel.NewAppSettings = appSettings.ToString();
                 log.LogInformation("NewSettings : " + BastionModel.NewAppSettings);
                //}
            }
            catch (Exception ex)
            {
                log.LogInformation("Error :" + ex.Message.ToString());
                throw ex;
            }
        }


        public static async void UpdateAppSettingsWithSchedules(ILogger log)
        {
            try
            {
                log.LogInformation("Before getApplicationSettings");

                await AddAppSettings.getApplicationSettings(log);
                log.LogInformation("After getApplicationSettings");

                log.LogInformation("Before BuildAppSettingsWithSchedules");
                await AddAppSettings.BuildAppSettingsWithSchedules(log);
                log.LogInformation("After BuildAppSettingsWithSchedules");

                log.LogInformation("Before AddApplicationSettings");
                await AddAppSettings.AddApplicationSettings(log);
                log.LogInformation("After AddApplicationSettings");

               // await getApplicationSettings(log);
               // await BuildAppSettingsWithSchedules(log);
               // await AddApplicationSettings(log);
            }
            catch (Exception ex)
            {
                log.LogInformation("Error :" + ex.Message.ToString());
                throw ex;
            }
        }

    }
}
