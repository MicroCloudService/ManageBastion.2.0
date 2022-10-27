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



namespace ManageBastion2
{
    static class VNet
    {
        public static async Task getVNetDetails()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://management.azure.com/subscriptions/" + BastionModel.SubscriptionID + "/resourcegroups/" + BastionModel.ResourceGroup + "/providers/Microsoft.Network/virtualNetworks/t/" + BastionModel.VNet + "?api-version=2021-03-01?a");
            //GET https://management.azure.com/subscriptions/subid/resourceGroups/rg1/providers/Microsoft.Network/virtualNetworks/test-vnet?api-version=2021-03-01
            client.DefaultRequestHeaders.Accept.Clear();

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + BastionModel.AccessToken);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, client.BaseAddress);

            var response = await Common.MakeRequestAsync(request, client);
            //            var body = $"{{\"location\": \"{BastionModel.Location}\"}}";
            //var content = new StringContent(body, Encoding.UTF8, "application/json");
            //request.Content = content;
            //var response = await MakeRequestAsync(request, client);
            BastionModel.Response = response;
        }
    }
}
