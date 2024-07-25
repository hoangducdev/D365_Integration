using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Sample.Integration.AzureFunctionApp.Constant;
using Sample.Integration.AzureFunctionApp.Interfaces;
using Sample.Integration.AzureFunctionApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sample.Integration.AzureFunctionApp.Functions
{
    public class FileHttpTrigger
    {
        private readonly ILogger<ServiceBusFileEventFunction> _logger;
        private readonly IDataverseService _dataverseService;
        private readonly IAppSettings _settings;
        private readonly IHttpClientFactory _httpClientFactory;

        public FileHttpTrigger(IAppSettings settings, ILogger<ServiceBusFileEventFunction> logger, IDataverseService dataverseService)
        {
            _settings = settings;
            _logger = logger;
            _dataverseService = dataverseService;
        }

        [FunctionName("HttpTrigger")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { Message = $" {ex.Message}" })
                {
                    StatusCode = 500
                };
            }

            return new ObjectResult(new { Message = $"ok" })
            {
                StatusCode = 200
            };
        }
    }
}