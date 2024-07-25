using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;
using System;
using System.Net.Http;

using Sample.Integration.AzureFunctionApp.Interfaces;

namespace Sample.Integration.AzureFunctionApp.Services
{
    public class ConnectionService : IConnectionService
    {
        private readonly IAppSettings _appSettings;
        private readonly ILogger<ConnectionService> _logger;
        private readonly ITokenCache _tokenCache;
        private readonly IHttpClientFactory _httpClientFactory;
        public ConnectionService(IAppSettings appSettings, ILogger<ConnectionService> logger, ITokenCache tokenCache, IHttpClientFactory httpClientFactory)
        {
            this._appSettings = appSettings;
            this._logger = logger;
            this._tokenCache = tokenCache;
            _httpClientFactory = httpClientFactory;
        }

        public ServiceClient CreateServiceClient()
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 65000;
            System.Threading.ThreadPool.SetMinThreads(100, 100);
            System.Net.ServicePointManager.Expect100Continue = false;
            System.Net.ServicePointManager.UseNagleAlgorithm = false;

            if (string.IsNullOrWhiteSpace(_appSettings.D365BasePath))
                throw new ArgumentNullException(nameof(_appSettings.D365BasePath));

            if (string.IsNullOrWhiteSpace(_appSettings.AppKeyD365))
                throw new ArgumentNullException(nameof(_appSettings.D365AppId));

            if (string.IsNullOrWhiteSpace(_appSettings.AppSecretD365))
                throw new ArgumentNullException(nameof(_appSettings.D365AppSecret));

            try
            {
                ServiceClient serviceClient = new($"AuthType=ClientSecret;Url={_appSettings.D365BasePath};ClientId={_appSettings.D365AppId};ClientSecret={_appSettings.D365AppSecret}");

                if (serviceClient.IsReady)
                {
                    return serviceClient;
                }
                else
                {
                    if (serviceClient.LastError.Equals("Unable to login to Microsoft Dataverse"))
                    {
                        _logger.LogError("Check the connection string values in app setting/key vault.");
                        throw new Exception(serviceClient.LastError);
                    }
                    else
                    {
                        _logger.LogError("Check the connection string values in app setting/key vault.");
                        throw serviceClient.LastException;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
