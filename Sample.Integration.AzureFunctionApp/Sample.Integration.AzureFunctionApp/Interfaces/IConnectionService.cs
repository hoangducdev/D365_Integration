using Microsoft.PowerPlatform.Dataverse.Client;

using System.Threading.Tasks;


namespace Sample.Integration.AzureFunctionApp.Interfaces
{
    public interface IConnectionService
    {
        ServiceClient CreateServiceClient();
    }
}
