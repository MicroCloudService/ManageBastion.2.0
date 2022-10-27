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
    public static class Authorization
    {
    public static void GetAuthorizationToken()
    {
        ClientCredential cc = new ClientCredential(BastionModel.SPN, BastionModel.SPNKEY);
        var context = new AuthenticationContext("https://login.microsoftonline.com/" + BastionModel.TenantID);
        var result = context.AcquireTokenAsync("https://management.azure.com/", cc);
        if (result == null)
        {
            throw new InvalidOperationException("Failed to obtain the Access token");
        }
        BastionModel.AccessToken = result.Result.AccessToken;
    }
    }


}
