using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Sample.Integration.AzureFunctionApp.Interfaces;
using Sample.Integration.AzureFunctionApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sample.Integration.AzureFunctionApp.Functions
{
    public class ServiceBusFileEventFunction
    {
        private readonly ILogger<ServiceBusFileEventFunction> _logger;
        private readonly IDataverseService _dataverseService;
        private readonly IAppSettings _settings;
        private readonly IHttpClientFactory _httpClientFactory;

        public ServiceBusFileEventFunction(IAppSettings settings, ILogger<ServiceBusFileEventFunction> logger, IDataverseService dataverseService, IHttpClientFactory httpClientFactory)
        {
            _settings = settings;
            _logger = logger;
            _dataverseService = dataverseService;
            _httpClientFactory = httpClientFactory;
        }

        [FunctionName("ServiceBusSampleFunction")]
        public async Task Run([ServiceBusTrigger("", Connection = "ServiceBusConnectionString")] ServiceBusReceivedMessage[] messages)
        {
            var exceptions = new List<Exception>();
            await Task.Yield();
            Parallel.ForEach(messages, async message =>
            {
                try
                {
                    ServiceBusInput serviceBusInput = JsonConvert.DeserializeObject<ServiceBusInput>(message.Body.ToString());
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            });

            if (exceptions.Count > 0)
            {
                foreach (var exception in exceptions)
                {
                    _logger.LogError(exception.Message, $@"Inner Excetion: {exception.InnerException}. Stack trace: {exception.StackTrace}");
                }
                throw new AggregateException(exceptions);
            }

            if (exceptions.Count == 1)
            {
                foreach (var exception in exceptions)
                {
                    _logger.LogError(exception.Message, $@"Inner Excetion: {exception.InnerException}. Stack trace: {exception.StackTrace}");
                }
                throw exceptions.Single();
            }
        }
    }
}
