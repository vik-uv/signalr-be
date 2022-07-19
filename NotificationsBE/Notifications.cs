using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using NotificationsBE.Models;

namespace NotificationsBE
{
    public static class Notifications
    {
        [OpenApiOperation(operationId: nameof(GetHomePage), tags: new[] { "signalr" }, Summary = "Gets the test HTML page", Description = "This gets the test HTML page.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/html", bodyType: typeof(string), Summary = "The response", Description = "This returns the response")]
        [FunctionName("index")]
        public static IActionResult GetHomePage([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req, ExecutionContext context, ILogger logger)
        {
            var path = Path.Combine(context.FunctionAppDirectory, "content", "index.html");
            return new ContentResult
            {
                Content = File.ReadAllText(path),
                ContentType = "text/html",
            };
        }

        [OpenApiOperation(operationId: nameof(Negotiate), tags: new[] { "signalr" }, Summary = "Gets SignalR params", Description = "This gets the SignalR params.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/json", bodyType: typeof(SignalRConnectionInfo), Summary = "The response", Description = "This returns the response")]
        [FunctionName("negotiate")]
        public static SignalRConnectionInfo Negotiate(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req,
            [SignalRConnectionInfo(HubName = "serverless")] SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }

        [OpenApiOperation(operationId: nameof(ConnectionOk), tags: new[] { "connection" }, Summary = "Gets OK text", Description = "This gets OK text if everytinh id OK.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/json", bodyType: typeof(string), Summary = "The response", Description = "This returns the response")]
        [FunctionName(nameof(ConnectionOk))]
        public static string ConnectionOk(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            return "OK";
        }


        [FunctionName(nameof(MessageChanged))]
        public static async Task MessageChanged([CosmosDBTrigger(
            databaseName: "%DatabaseName%",
            collectionName: "%CollectionName%",
            ConnectionStringSetting = "CosmosDbConnectionString",
            LeaseCollectionName = "%CollectionNameLease%",
            LeaseCollectionPrefix = "Notifications",
            CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Microsoft.Azure.Documents.Document> documents,
            [SignalR(HubName = "serverless")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            if (documents != null && documents.Count > 0)
            {
                await signalRMessages.AddAsync(
                    new SignalRMessage
                    {
                        Target = "newMessage",
                        Arguments = documents.Select(doc => JsonConvert.DeserializeObject<Message>(doc.ToString())).ToArray()
                    });
            }
        }
    }
}
