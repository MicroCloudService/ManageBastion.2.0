//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System;
using System.Linq;

using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ManageBastion2
{
    // Custom Resource Table Entity
    public class CustomResource : TableEntity
    {
        public string Data { get; set; }
    }

    public static class BastionCronSchedule
    {
        [FunctionName("BastionCronSchedule")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "put", "post", "delete", Route = null)] HttpRequestMessage req,
            [Table("customResource")] CloudTable tableStorage,
            ILogger log, ExecutionContext context)
        {

            await GetBastionSettings.GetSettings(log, context);

            // Get the unique Azure request path from request headers.
            //if (!req.Headers.ContainsKey("x-ms-customproviders-requestpath"))
            //{
            //    return new BadRequestResult();
            //}

            var requestPath = req.Headers.GetValues("x-ms-customproviders-requestpath").First();
            log.LogInformation("requestPath" + requestPath);
            log.LogInformation($"The Custom Provider Function received a request '{req.Method}' for resource '{requestPath}'.");

            var requestBody = await req.Content.ReadAsStringAsync();
            log.LogInformation("req.Body" + requestBody);

            //log.LogInformation("req.message" + req.Me);
            //if (!string.IsNullOrEmpty(requestBody))
            //{
            //    dynamic data = JsonConvert.DeserializeObject(requestBody);
            //}




            //var referer = req.Headers["Referer"].ToString();
            //log.LogInformation($"Referer : {referer}");

            // Determines if it is a collection level call or action.
            var isResourceRequest = requestPath.Split('/').Length % 2 == 1;

            log.LogInformation("isResourceRequest" + isResourceRequest);

            var azureResourceId = isResourceRequest ?
                ResourceId.FromString(requestPath) :
                ResourceId.FromString($"{requestPath}/");
            log.LogInformation("isResourceRequest" + isResourceRequest);

            // Create the Partition Key and Row Key
            log.LogInformation("azureResourceId.SubscriptionId : " + azureResourceId.SubscriptionId);
            log.LogInformation("azureResourceId.ResourceGroupName : " + azureResourceId.ResourceGroupName);
            log.LogInformation("azureResourceId.Parent.Name : " + azureResourceId.Parent.Name);
            log.LogInformation("azureResourceId.Name : " + azureResourceId.Name);

            var partitionKey = $"{azureResourceId.SubscriptionId}:{azureResourceId.ResourceGroupName}:{azureResourceId.Parent.Name}";
            log.LogInformation("partitionKey : " + partitionKey);

            var rowKey = $"{azureResourceId.FullResourceType.Replace('/', ':')}:{azureResourceId.Name}";
            log.LogInformation("rowKey : " + rowKey);

            switch (req.Method)
            {
                // Action request for an custom action.
                case HttpMethod m when m == HttpMethod.Post && !isResourceRequest:
                    log.LogInformation("Inside POST");
                    return await TriggerCustomAction(
                        requestMessage: req,
                        tableStorage: tableStorage,
                        partitionKey: partitionKey,
                        rowKey: rowKey,
                        customResourceType: azureResourceId.Parent.Name,
                         log: log);

                // Enumerate request for all custom resources.
                case HttpMethod m when m == HttpMethod.Get && !isResourceRequest:
                    log.LogInformation("Inside GET All");
                    return await EnumerateAllCustomResources(
                        tableStorage: tableStorage,
                        partitionKey: partitionKey,
                        resourceType: rowKey);

                // Retrieve request for a custom resource.
                case HttpMethod m when m == HttpMethod.Get && isResourceRequest:
                    log.LogInformation("Inside Get Single");
                    return await RetrieveCustomResource(
                        tableStorage: tableStorage,
                        partitionKey: partitionKey,
                        rowKey: rowKey);

                // Create request for a custom resource.
                case HttpMethod m when m == HttpMethod.Put && isResourceRequest:
                    log.LogInformation("Inside PUT");
                    var ret = await CreateCustomResource(
                        requestMessage: req,
                        tableStorage: tableStorage,
                        azureResourceId: azureResourceId,
                        partitionKey: partitionKey,
                        rowKey: rowKey,
                         log: log);
                    log.LogInformation("Result Return :" + ret.ToString());
                    return ret;

                // Remove request for a custom resource.
                case HttpMethod m when m == HttpMethod.Delete && isResourceRequest:
                    log.LogInformation("Inside DELETE");
                    return await RemoveCustomResource(
                        tableStorage: tableStorage,
                        partitionKey: partitionKey,
                        rowKey: rowKey,
                        cronCallState: azureResourceId.Name,
                         log: log);

                // Invalid request recieved.
                default:
                    HttpRequestMessage resp = new HttpRequestMessage();
                    var badRequestResponse = resp.CreateResponse(HttpStatusCode.BadRequest);
                    return badRequestResponse;
            }
        }

        /// <summary>
        /// Triggers a custom action with some side effect.
        /// </summary>
        /// <param name="requestMessage">The http request message.</param>
        /// <returns>The http response result of the custom action.</returns>
        public static async Task<HttpResponseMessage> TriggerCustomAction(HttpRequestMessage requestMessage, CloudTable tableStorage, string partitionKey, string rowKey, string customResourceType,
            ILogger log)
        {
            try
            {
                log.LogInformation("Inside TriggerCustomAction: rowKey=" + rowKey);
                var rowKeyCcaller = rowKey.Split(':');
                string[] array = rowKeyCcaller.Take(rowKeyCcaller.Length - 1).ToArray();
                var caller = array.Last();
                log.LogInformation("Caller Method :" + caller);

                var newarray = array.Take(array.Length - 1).ToArray();

                var custombuilder = ":" + customResourceType;

                var buildNewRowKey = String.Join(":", newarray) + custombuilder;

                //var partitionkey = "e9b63d88-2fb5-49f6-8d1d-2ef618a7f43c:mrg-17mar9-20220317182934:public";
                var partitionkeyarray = partitionKey.Split(':');

                string[] partitionkeynewarray = partitionkeyarray.Take(partitionkeyarray.Length - 1).ToArray();

                var buildNewPartitionKey = String.Join(":", partitionkeynewarray) + ":public";

                // Console.WriteLine("MEthod : " + array.Last());

                var myCustomActionRequest = caller;// await requestMessage.Content.ReadAsStringAsync();
                log.LogInformation("myCustomActionRequest" + myCustomActionRequest);
                var createResponse = requestMessage.CreateResponse(HttpStatusCode.OK);
                //createResponse.Content = new StringContent(JObject.Parse(myCustomActionRequest).ToString(), System.Text.Encoding.UTF8, "application/json");

                var tableQuery = TableOperation.Retrieve<CustomResource>(buildNewPartitionKey, buildNewRowKey);

                var existingCustomResource = (CustomResource)(await tableStorage.ExecuteAsync(tableQuery)).Result;
                log.LogInformation("existingCustomResource" + existingCustomResource);

                HttpRequestMessage resp = new HttpRequestMessage();
                JToken cron = "";

                string cronNotDisabled = "0";


                switch (myCustomActionRequest)
                {
                    // Action request for an custom action.
                    case "setoffschedule":
                        if (existingCustomResource != null)
                        {
                            log.LogInformation("existingCustomResource.Data" + existingCustomResource.Data);
                            //var createResponse = resp.CreateResponse(HttpStatusCode.OK);
                            //createResponse.Content = new StringContent(existingCustomResource.Data.ToString(), System.Text.Encoding.UTF8, "application/json");
                            // return createResponse; // new OkObjectResult(new StringContent(existingCustomResource.Data, System.Text.Encoding.UTF8, "application/json"));
                            var fg = JObject.Parse(existingCustomResource.Data);
                            cron = fg.SelectToken("$..properties.OffSchedule");
                            log.LogInformation("cron." + cron);
                        }
                        log.LogInformation("Inside setoffschedule");
                        BastionModel.OffTimerSchedule = cron.ToString();
                        BastionModel.OffTimerScheduleDisabled = cronNotDisabled;
                        AddAppSettings.UpdateAppSettingsWithSchedules(log);
                        return createResponse;

                    case "setonschedule":
                        if (existingCustomResource != null)
                        {
                            log.LogInformation("existingCustomResource.Data" + existingCustomResource.Data);
                            //var createResponse = resp.CreateResponse(HttpStatusCode.OK);
                            //createResponse.Content = new StringContent(existingCustomResource.Data.ToString(), System.Text.Encoding.UTF8, "application/json");
                            // return createResponse; // new OkObjectResult(new StringContent(existingCustomResource.Data, System.Text.Encoding.UTF8, "application/json"));
                            var fg = JObject.Parse(existingCustomResource.Data);
                            cron = fg.SelectToken("$..properties.OnSchedule");
                            log.LogInformation("cron" + cron);
                        }
                        log.LogInformation("Inside setonschedule");
                        BastionModel.OnTimerSchedule = cron.ToString();
                        BastionModel.OnTimerScheduleDisabled = cronNotDisabled;
                        AddAppSettings.UpdateAppSettingsWithSchedules(log);
                        return createResponse;
                }



                //var createResponse = requestMessage.CreateResponse(HttpStatusCode.OK);
                // createResponse.Content = new StringContent(JObject.Parse(myCustomActionRequest).ToString(), System.Text.Encoding.UTF8, "application/json");
                //return createResponse;

                return myCustomActionRequest != string.Empty ?
                   createResponse :
                    null;
            }
            catch (Exception ex)
            {
                String exMessage = (ex.InnerException != null)
                      ? ex.InnerException.Message
                      : ex.Message.ToString();

                var createResponse = requestMessage.CreateResponse(HttpStatusCode.InternalServerError, exMessage.ToString());
                createResponse.Content = new StringContent(exMessage.ToString(), System.Text.Encoding.UTF8, "application/json");
                return createResponse; // new OkObjectResult(responseMessage);
            }
        }

        /// <summary>
        /// Enumerates all the stored custom resources for a given type.
        /// </summary>
        /// <param name="tableStorage">The Azure Storage Account table.</param>
        /// <param name="partitionKey">The partition key for storage. This is the custom resource provider id.</param>
        /// <param name="resourceType">The resource type of the enumeration.</param>
        /// <returns>The http response containing a list of resources stored under 'value'.</returns>
        public static async Task<HttpResponseMessage> EnumerateAllCustomResources(CloudTable tableStorage, string partitionKey, string resourceType)
        {
            // Generate upper bound of the query.
            var rowKeyUpperBound = new StringBuilder(resourceType);
            rowKeyUpperBound[rowKeyUpperBound.Length - 1]++;

            // Create the enumeration query.
            var enumerationQuery = new TableQuery<CustomResource>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey),
                    TableOperators.And,
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThan, resourceType),
                        TableOperators.And,
                        TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThan, rowKeyUpperBound.ToString()))));

            var customResources = (await tableStorage.ExecuteQuerySegmentedAsync(enumerationQuery, null))
                .ToList().Select(customResource => JToken.Parse(customResource.Data));


            HttpRequestMessage resp = new HttpRequestMessage();
            var enumerationResponse = resp.CreateResponse(HttpStatusCode.OK);
            enumerationResponse.Content = new StringContent(new JObject(new JProperty("value", customResources)).ToString(), System.Text.Encoding.UTF8, "application/json");

            //var enumerationResponse = new OkObjectResult(new StringContent(new JObject(new JProperty("value", customResources)).ToString(), System.Text.Encoding.UTF8, "application/json"));
            return enumerationResponse;
        }

        /// <summary>
        /// Retrieves a custom resource.
        /// </summary>
        /// <param name="tableStorage">The Azure Storage Account table.</param>
        /// <param name="partitionKey">The partition key for storage. This is the custom resource provider id.</param>
        /// <param name="rowKey">The row key for storage. This is '{resourceType}:{customResourceName}'.</param>
        /// <returns>The http response containing the existing custom resource.</returns>
        public static async Task<HttpResponseMessage> RetrieveCustomResource(CloudTable tableStorage, string partitionKey, string rowKey)
        {
            // Attempt to retrieve the Existing Stored Value
            var tableQuery = TableOperation.Retrieve<CustomResource>(partitionKey, rowKey);
            var existingCustomResource = (CustomResource)(await tableStorage.ExecuteAsync(tableQuery)).Result;
            HttpRequestMessage resp = new HttpRequestMessage();
            if (existingCustomResource != null)
            {

                var createResponse = resp.CreateResponse(HttpStatusCode.OK);
                createResponse.Content = new StringContent(existingCustomResource.Data.ToString(), System.Text.Encoding.UTF8, "application/json");
                return createResponse; // new OkObjectResult(new StringContent(existingCustomResource.Data, System.Text.Encoding.UTF8, "application/json"));
            }

            var createResponse2 = resp.CreateResponse(HttpStatusCode.NotFound);
            return createResponse2;// new NotFoundResult();
        }

        /// <summary>
        /// Creates a custom resource and saves it to table storage.
        /// </summary>
        /// <param name="requestMessage">The http request message.</param>
        /// <param name="tableStorage">The Azure Storage Account table.</param>
        /// <param name="azureResourceId">The parsed Azure resource Id.</param>
        /// <param name="partitionKey">The partition key for storage. This is the custom resource provider id.</param>
        /// <param name="rowKey">The row key for storage. This is '{resourceType}:{customResourceName}'.</param>
        /// <returns>The http response containing the created custom resource.</returns>
        public static async Task<HttpResponseMessage> CreateCustomResource(HttpRequestMessage requestMessage, CloudTable tableStorage, ResourceId azureResourceId, string partitionKey, string rowKey, ILogger log)
        {
            string responseMessage = "";
            try
            {
                // Construct the new resource from the request body and adds the Azure Resource Manager fields.
                //var requestBody = await new StreamReader(requestMessage.Body).ReadToEndAsync();
                var myCustomResource = JObject.Parse(await requestMessage.Content.ReadAsStringAsync());
                //var myCustomResource = JObject.Parse(requestBody);
                log.LogInformation("myCustomResource : " + myCustomResource);
                myCustomResource["name"] = azureResourceId.Name;
                myCustomResource["type"] = azureResourceId.FullResourceType;
                log.LogInformation("azureResourceId.FullResourceType : " + azureResourceId.FullResourceType);

                myCustomResource["id"] = azureResourceId.Id;
                log.LogInformation("azureResourceId.Id : " + azureResourceId.Id);
                //myCustomResource["CRON"] = new JObject(new JProperty("ScheduleOn", "0 0 9 * * MON"), new JProperty("ScheduleOff", "0 0 10 * * SUN"));

                // Save the resource into storage.
                var insertOperation = TableOperation.InsertOrReplace(
                new CustomResource
                {
                    PartitionKey = partitionKey,
                    RowKey = rowKey,
                    Data = myCustomResource.ToString(),
                });
                await tableStorage.ExecuteAsync(insertOperation);
                var createResponse = requestMessage.CreateResponse(HttpStatusCode.OK);
                createResponse.Content = new StringContent(myCustomResource.ToString(), System.Text.Encoding.UTF8, "application/json");

                return createResponse; // new OkObjectResult(new StringContent(myCustomResource.ToString(), System.Text.Encoding.UTF8, "application/json"));
            }
            catch (Exception ex)
            {
                var createResponse = requestMessage.CreateResponse(HttpStatusCode.BadRequest);
                createResponse.Content = new StringContent(ex.Message.ToString(), System.Text.Encoding.UTF8, "application/json");
                return createResponse; // new OkObjectResult(responseMessage);
            }

        }

        /// <summary>
        /// Removes an existing custom resource.
        /// </summary>
        /// <param name="tableStorage">The Azure Storage Account table.</param>
        /// <param name="partitionKey">The partition key for storage. This is the custom resource provider id.</param>
        /// <param name="rowKey">The row key for storage. This is '{resourceType}:{customResourceName}'.</param>
        /// <returns>The http response containing the result of the delete.</returns>
        public static async Task<HttpResponseMessage> RemoveCustomResource(CloudTable tableStorage, string partitionKey, string rowKey, string cronCallState,
            ILogger log)
        {
            // Attempt to retrieve the Existing Stored Value
            var tableQuery = TableOperation.Retrieve<CustomResource>(partitionKey, rowKey);
            var existingCustomResource = (CustomResource)(await tableStorage.ExecuteAsync(tableQuery)).Result;
            string cronDisabled = "1";

            if (existingCustomResource != null)
            {
                var deleteOperation = TableOperation.Delete(existingCustomResource);
                await tableStorage.ExecuteAsync(deleteOperation);
            }

            if(cronCallState=="CRONOFF")
            {
                log.LogInformation("Inside setoffschedule");
                BastionModel.OffTimerScheduleDisabled = cronDisabled;
                AddAppSettings.UpdateAppSettingsWithSchedules(log);

            }

            if (cronCallState == "CRONON")
            {
                log.LogInformation("Inside setoffschedule");
                BastionModel.OffTimerScheduleDisabled = cronDisabled;
                AddAppSettings.UpdateAppSettingsWithSchedules(log);

            }
                HttpRequestMessage resp = new HttpRequestMessage();
            if (existingCustomResource != null)
            {


                var okResponse = resp.CreateResponse(HttpStatusCode.OK);
                return okResponse;
            }
            var noContentResponse = resp.CreateResponse(HttpStatusCode.NoContent);
            return noContentResponse;
        }
    }
}