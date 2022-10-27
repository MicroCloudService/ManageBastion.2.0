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
using Newtonsoft.Json;


namespace ManageBastion2
{
    static class PublicIP
    {
        public static async Task getPublicIPDetails()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://management.azure.com/subscriptions/" + BastionModel.SubscriptionID + "/resourcegroups/" + BastionModel.ResourceGroup + "providers/Microsoft.Network/publicIPAddresses/" + BastionModel.PublicIP + "?api-version=2021-03-01");
            //GET https://management.azure.com/subscriptions/subid/resourceGroups/rg1/providers/Microsoft.Network/publicIPAddresses/testDNS-ip?api-version=2021-03-01
            client.DefaultRequestHeaders.Accept.Clear();

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + BastionModel.AccessToken);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, client.BaseAddress);

            var json = await Common.MakeRequestAsync(request, client);
            dynamic results = JsonConvert.DeserializeObject<dynamic>(json);

            var id = results.Id;
            var name = results.Name;
        }
    }
}
