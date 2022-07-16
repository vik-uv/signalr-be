using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NotificationsBE.DB;
using System.Threading;
using NotificationsBE.Models;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Azure.Documents;
using System.Linq;
using System.Net;
using Microsoft.OpenApi.Models;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;

namespace NotificationsBE.Functions
{
    public class MessageFunctions
    {
        private readonly ILogger<MessageFunctions> _logger;
        private readonly IMessageService _messageService;

        public MessageFunctions(
            ILogger<MessageFunctions> logger,
            IMessageService messageService)
        {
            _logger = logger;
            _messageService = messageService;
        }

        #region CRUD
        [OpenApiOperation(operationId: nameof(GetMessagesList), tags: new[] { "message" }, Summary = "Get messages list", Description = "This gets messages list.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "count", In = ParameterLocation.Path, Required = true, Type = typeof(int), Summary = "Count of records to get", Description = "Count of records to get", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/json", bodyType: typeof(Message[]), Summary = "List of messages", Description = "This returns list of messages")]
        [FunctionName(nameof(GetMessagesList))]
        public async Task<IActionResult> GetMessagesList(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "messages/{count}")] HttpRequest req,
            int? count,
            CancellationToken cancellationToken)
        {
            IActionResult result;

            try
            {
                var messages = await _messageService.GetMessagesAsync(count ?? 100, cancellationToken);
                result = new OkObjectResult(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal Server Error. Exception thrown: {ex.Message}");
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return result;
        }

        [OpenApiOperation(operationId: nameof(GetMessage), tags: new[] { "message" }, Summary = "Get message by id", Description = "This gets message by id.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "Message ID", Description = "Message ID", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/json", bodyType: typeof(Message), Summary = "Message", Description = "This returns message")]
        [FunctionName(nameof(GetMessage))]
        public async Task<IActionResult> GetMessage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "message/{id}")] HttpRequest req,
            string id,
            CancellationToken cancellationToken)
        {
            IActionResult result;

            try
            {
                var messages = await _messageService.GetMessageAsync(id, cancellationToken);
                result = new OkObjectResult(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal Server Error. Exception thrown: {ex.Message}");
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return result;
        }

        [OpenApiOperation(operationId: nameof(AddMessage), tags: new[] { "message" }, Summary = "Add a message", Description = "This adds a message.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Message), Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/json", bodyType: typeof(StatusCodeResult), Summary = "Status", Description = "This returns operation status")]
        [FunctionName(nameof(AddMessage))]
        public async Task<IActionResult> AddMessage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "message")] HttpRequest req,
            CancellationToken cancellationToken)
        {
            IActionResult result;

            try
            {
                var incomingRequest = await new StreamReader(req.Body).ReadToEndAsync();
                var request = JsonConvert.DeserializeObject<Message>(incomingRequest);
                var message = new Message
                {
                    Id = Guid.NewGuid().ToString(),
                    DateTime = request.DateTime ?? DateTime.UtcNow,
                    Group = request.Group,
                    Content = request.Content
                };

                await _messageService.AddMessageAsync(message, cancellationToken);
                result = new StatusCodeResult(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal Server Error. Exception thrown: {ex.Message}");
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return result;
        }

        [OpenApiOperation(operationId: nameof(UpdateMessage), tags: new[] { "message" }, Summary = "Update a message", Description = "This updates a message.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Message), Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/json", bodyType: typeof(StatusCodeResult), Summary = "Status", Description = "This returns operation status")]
        [FunctionName(nameof(UpdateMessage))]
        public async Task<IActionResult> UpdateMessage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "message")] HttpRequest req,
            CancellationToken cancellationToken)
        {
            IActionResult result;

            try
            {
                var incomingRequest = await new StreamReader(req.Body).ReadToEndAsync();
                var request = JsonConvert.DeserializeObject<Message>(incomingRequest);

                var message = await _messageService.GetMessageAsync(request.Id, cancellationToken);
                if (message == null)
                {
                    _logger.LogWarning($"Message with id: {request.Id} doesn't exist.");
                    result = new StatusCodeResult(StatusCodes.Status404NotFound);
                }
                else
                {
                    message = new Message
                    {
                        Id = Guid.NewGuid().ToString(),
                        DateTime = request.DateTime ?? DateTime.UtcNow,
                        Group = request.Group,
                        Content = request.Content
                    };

                    await _messageService.UpdateMessageAsync(message, cancellationToken);
                    result = new StatusCodeResult(StatusCodes.Status200OK);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal Server Error. Exception thrown: {ex.Message}");
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return result;
        }

        [OpenApiOperation(operationId: nameof(DeleteMessage), tags: new[] { "message" }, Summary = "Delete a message", Description = "This deletes a message.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "Message ID", Description = "Message ID", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/json", bodyType: typeof(StatusCodeResult), Summary = "Status", Description = "This returns operation status")]
        [FunctionName(nameof(DeleteMessage))]
        public async Task<IActionResult> DeleteMessage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "message/{id}")] HttpRequest req,
            string id,
            CancellationToken cancellationToken)
        {
            IActionResult result;

            try
            {
                var message = await _messageService.GetMessageAsync(id, cancellationToken);
                if (message == null)
                {
                    _logger.LogWarning($"Message with id: {id} doesn't exist.");
                    result = new StatusCodeResult(StatusCodes.Status404NotFound);
                }
                else
                {
                    await _messageService.DeleteMessageAsync(id, cancellationToken);
                    result = new StatusCodeResult(StatusCodes.Status200OK);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal Server Error. Exception thrown: {ex.Message}");
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return result;
        }
        #endregion
    }
}
