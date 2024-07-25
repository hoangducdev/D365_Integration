using System;
using System.IO;
using System.Net.Http;
using System.Runtime;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Sample.Integration.AzureFunctionApp.Adapter;
using Sample.Integration.AzureFunctionApp.Constant;
using Sample.Integration.AzureFunctionApp.Interfaces;
using Sample.Integration.AzureFunctionApp.Models;

namespace Sample.Integration.AzureFunctionApp.Functions
{
    public class BlobFunction
    {
        private readonly ILogger<BlobFunction> _logger;
        private readonly IDataverseService _dataverseService;
        private readonly IAppSettings _settings;
        private readonly IHttpClientFactory _httpClientFactory;

        public BlobFunction(IAppSettings settings, ILogger<BlobFunction> logger, IDataverseService dataverseService, IHttpClientFactory httpClientFactory)
        {
            _settings = settings;
            _logger = logger;
            _dataverseService = dataverseService;
            _httpClientFactory = httpClientFactory;
        }


        [FunctionName("BlobFunction")]
        public async Task Run([BlobTrigger("sample/{name}", Connection = "sampleconnection")] Stream myBlob, string name, ILogger log)
        {
            try
            {
                log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
                byte[] test = Helper.Helper.ReadToEnd(myBlob);
                MemoryStream stream = new(test);
            } 
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
