//using System;
//using System.IO;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Azure.WebJobs;
//using Microsoft.Azure.WebJobs.Extensions.Http;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Logging;
//using Newtonsoft.Json;
//using System;
//using System.Net;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Configuration;
//using System.Text;
//using System.Threading;
//using System.Globalization;
//using System.Collections.Generic;
//using Microsoft.Azure.WebJobs;
//using Microsoft.Azure.WebJobs.Host;
//using Microsoft.WindowsAzure.Storage.Table;
//using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using System.Linq;


//namespace ManageBastion2
//{
//    // Custom Resource Table Entity
//public class CustomResource : TableEntity
//{
//    public string Data { get; set; }
//}
//    public static class CreatingCustomResource
//    {
//        [FunctionName("CreatingCustomResource")]
//        public static async Task<IActionResult> Run(
//            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
//            ILogger log, CloudTable tableStorage)
//        {
//            var requestPath = string.Empty;

//            // Get the unique Azure request path from request headers.
//            requestPath = req.Headers["x-ms-customproviders-requestpath"];
           

//            log.LogInformation($"The Custom Provider Function received a request '{req.Method}' for resource '{requestPath}'.");

//            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
//            dynamic data = JsonConvert.DeserializeObject(requestBody);
//            var referer = req.Headers["Referer"].ToString();
//            log.LogInformation("Referer : " +referer);

//            // Determines if it is a collection level call or action.
//            var isResourceRequest = requestPath.Split('/').Length % 2 == 1;
//            var azureResourceId = isResourceRequest ?
//                ResourceId.FromString(requestPath) :
//                ResourceId.FromString($"{requestPath}/");

//            // Create the Partition Key and Row Key
//            var partitionKey = $"{azureResourceId.SubscriptionId}:{azureResourceId.ResourceGroupName}:{azureResourceId.Parent.Name}";
//            var rowKey = $"{azureResourceId.FullResourceType.Replace('/', ':')}:{azureResourceId.Name}";
//            var methodType = req.Method;
//            switch (methodType)
//            {
//                // Action request for an custom action.
//                //case string m when HttpMethod.Post.ToString() == methodType && !isResourceRequest:
//                //    return await TriggerCustomAction(requestMessage: req);

//                // Enumerate request for all custom resources.
//                case string m when HttpMethod.Get.ToString() == methodType && !isResourceRequest:
//                    return await EnumerateAllCustomResources(
//                        requestMessage: req,
//                        tableStorage: tableStorage,
//                        partitionKey: partitionKey,
//                        resourceType: rowKey);

//                // Retrieve request for a custom resource.
//                case string m when HttpMethod.Get.ToString() == methodType && isResourceRequest:
//                    return await RetrieveCustomResource(
//                        requestMessage: req,
//                        tableStorage: tableStorage,
//                        partitionKey: partitionKey,
//                        rowKey: rowKey);

//                // Create request for a custom resource.
//                case string m when HttpMethod.Put.ToString() == methodType && isResourceRequest:

//                    log.LogInformation("partitionKey:" + partitionKey);
//                    log.LogInformation("rowKey:" + rowKey);
//                    log.LogInformation("tableStorage:" + tableStorage);
//                    log.LogInformation("azureResourceId:" + azureResourceId);

//                    return await CreateCustomResource(
//                        requestMessage: req,
//                        tableStorage: tableStorage,
//                        azureResourceId: azureResourceId,
//                        partitionKey: partitionKey,
//                        rowKey: rowKey);

//                // Remove request for a custom resource.
//                case string m when HttpMethod.Delete.ToString() == methodType && isResourceRequest:
//                    return await RemoveCustomResource(
//                        requestMessage: req,
//                        tableStorage: tableStorage,
//                        partitionKey: partitionKey,
//                        rowKey: rowKey);

//                // Invalid request recieved.
//                default:
//                    return new BadRequestObjectResult(HttpStatusCode.BadRequest); 
//            }
//        }

//        /// <summary>
//        /// Triggers a custom action with some side effect.
//        /// </summary>
//        /// <param name="requestMessage">The http request message.</param>
//        /// <returns>The http response result of the custom action.</returns>
//        //public static async Task<IActionResult> TriggerCustomAction(HttpRequest requestMessage)
//        //{
//        //    var myCustomActionRequest = await requestMessage.Content.ReadAsStringAsync();

//        //    var actionResponse = requestMessage.CreateResponse(HttpStatusCode.OK);
//        //    actionResponse.Content = myCustomActionRequest != string.Empty ?
//        //        new StringContent(JObject.Parse(myCustomActionRequest).ToString(), System.Text.Encoding.UTF8, "application/json") :
//        //        null;
//        //    return actionResponse;
//        //}

//        /// <summary>
//        /// Enumerates all the stored custom resources for a given type.
//        /// </summary>
//        /// <param name="requestMessage">The http request message.</param>
//        /// <param name="tableStorage">The Azure Storage Account table.</param>
//        /// <param name="partitionKey">The partition key for storage. This is the custom resource provider id.</param>
//        /// <param name="resourceType">The resource type of the enumeration.</param>
//        /// <returns>The http response containing a list of resources stored under 'value'.</returns>
//        public static async Task<IActionResult> EnumerateAllCustomResources(HttpRequest requestMessage, CloudTable tableStorage, string partitionKey, string resourceType)
//        {
//            // Generate upper bound of the query.
//            var rowKeyUpperBound = new StringBuilder(resourceType);
//            rowKeyUpperBound[rowKeyUpperBound.Length - 1]++;

//            // Create the enumeration query.
//            var enumerationQuery = new TableQuery<CustomResource>().Where(
//                TableQuery.CombineFilters(
//                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey),
//                    TableOperators.And,
//                    TableQuery.CombineFilters(
//                        TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThan, resourceType),
//                        TableOperators.And,
//                        TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThan, rowKeyUpperBound.ToString()))));

//            var customResources = (await tableStorage.ExecuteQuerySegmentedAsync(enumerationQuery, null))
//                .ToList().Select(customResource => JToken.Parse(customResource.Data));

//            var enumerationResponse = new OkObjectResult(new StringContent(new JObject(new JProperty("value", customResources)).ToString(), System.Text.Encoding.UTF8, "application/json"));// requestMessage.CreateResponse(HttpStatusCode.OK);
//            //enumerationResponse.Content = new StringContent(new JObject(new JProperty("value", customResources)).ToString(), System.Text.Encoding.UTF8, "application/json");
//            return enumerationResponse;
//        }

//        /// <summary>
//        /// Retrieves a custom resource.
//        /// </summary>
//        /// <param name="requestMessage">The http request message.</param>
//        /// <param name="tableStorage">The Azure Storage Account table.</param>
//        /// <param name="partitionKey">The partition key for storage. This is the custom resource provider id.</param>
//        /// <param name="rowKey">The row key for storage. This is '{resourceType}:{customResourceName}'.</param>
//        /// <returns>The http response containing the existing custom resource.</returns>
//        public static async Task<IActionResult> RetrieveCustomResource(HttpRequest requestMessage, CloudTable tableStorage, string partitionKey, string rowKey)
//        {
//            // Attempt to retrieve the Existing Stored Value
//            var tableQuery = TableOperation.Retrieve<CustomResource>(partitionKey, rowKey);
//            var existingCustomResource = (CustomResource)(await tableStorage.ExecuteAsync(tableQuery)).Result;

//            ObjectResult retrieveResponse;// = IHttpActionResult;//= requestMessage.CreateResponse(
//            if (existingCustomResource != null)
//            {
//                 retrieveResponse = new OkObjectResult(new StringContent(existingCustomResource.Data, System.Text.Encoding.UTF8, "application/json"));
//            }
//            else
//            {
//                retrieveResponse = new NotFoundObjectResult(HttpStatusCode.NotFound);
//            };
//            return retrieveResponse;
//        }

//        /// <summary>
//        /// Creates a custom resource and saves it to table storage.
//        /// </summary>
//        /// <param name="requestMessage">The http request message.</param>
//        /// <param name="tableStorage">The Azure Storage Account table.</param>
//        /// <param name="azureResourceId">The parsed Azure resource Id.</param>
//        /// <param name="partitionKey">The partition key for storage. This is the custom resource provider id.</param>
//        /// <param name="rowKey">The row key for storage. This is '{resourceType}:{customResourceName}'.</param>
//        /// <returns>The http response containing the created custom resource.</returns>
//        public static async Task<IActionResult> CreateCustomResource(HttpRequest requestMessage, CloudTable tableStorage, ResourceId azureResourceId, string partitionKey, string rowKey)
//        {
//            // Construct the new resource from the request body and adds the Azure Resource Manager fields.
//            StreamReader reader = new StreamReader(requestMessage.Body, null);

//                var myCustomResource1 = await reader.ReadToEndAsync();
//            var myCustomResource = JObject.Parse(myCustomResource1); //await requestMessage.Content.ReadAsStringAsync());
//            myCustomResource["name"] = azureResourceId.Name;
//            myCustomResource["type"] = azureResourceId.FullResourceType;
//            myCustomResource["id"] = azureResourceId.Id;

//            // Save the resource into storage.
//            var insertOperation = TableOperation.InsertOrReplace(
//                new CustomResource
//                {
//                    PartitionKey = partitionKey,
//                    RowKey = rowKey,
//                    Data = myCustomResource.ToString(),
//                });
//            await tableStorage.ExecuteAsync(insertOperation);

//            var createResponse = new OkObjectResult(new StringContent(myCustomResource.ToString(), System.Text.Encoding.UTF8, "application/json"));
//            //var createResponse = requestMessage.CreateResponse(HttpStatusCode.OK);
//            //createResponse.Content = new StringContent(myCustomResource.ToString(), System.Text.Encoding.UTF8, "application/json");
//            return createResponse;
//        }

//        /// <summary>
//        /// Removes an existing custom resource.
//        /// </summary>
//        /// <param name="requestMessage">The http request message.</param>
//        /// <param name="tableStorage">The Azure Storage Account table.</param>
//        /// <param name="partitionKey">The partition key for storage. This is the custom resource provider id.</param>
//        /// <param name="rowKey">The row key for storage. This is '{resourceType}:{customResourceName}'.</param>
//        /// <returns>The http response containing the result of the delete.</returns>
//        public static async Task<IActionResult> RemoveCustomResource(HttpRequest requestMessage, CloudTable tableStorage, string partitionKey, string rowKey)
//        {
//            // Attempt to retrieve the Existing Stored Value
//            var tableQuery = TableOperation.Retrieve<CustomResource>(partitionKey, rowKey);
//            var existingCustomResource = (CustomResource)(await tableStorage.ExecuteAsync(tableQuery)).Result;

//            if (existingCustomResource != null)
//            {
//                var deleteOperation = TableOperation.Delete(existingCustomResource);
//                await tableStorage.ExecuteAsync(deleteOperation);
//            }
//            ObjectResult retrieveResponse;// = IHttpActionResult;//= requestMessage.CreateResponse(
//            if (existingCustomResource != null)
//            {
//                retrieveResponse = new OkObjectResult(HttpStatusCode.OK);
//            }
//            else
//            {
//                retrieveResponse = new OkObjectResult(HttpStatusCode.NoContent);
//            };
//            return retrieveResponse;
//        }

//    }


  
//}
