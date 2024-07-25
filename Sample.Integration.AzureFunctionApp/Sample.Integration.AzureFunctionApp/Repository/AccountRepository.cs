using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Sample.Integration.AzureFunctionApp.Constant;
using Sample.Integration.AzureFunctionApp.Interfaces;
using System;
using System.Threading.Tasks;

namespace Sample.Integration.AzureFunctionApp.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IConnectionService _connectionService;
        private readonly ServiceClient serviceClient;

        public AccountRepository(IConnectionService connectionService)
        {
            _connectionService = connectionService;
            serviceClient = _connectionService.CreateServiceClient();
        }
        public async Task<Guid> CreateAccount(Entity entity)
        {
            return await serviceClient.CreateAsync(entity);
        }

        public async Task DeleteAccount(Guid id)
        {
            await serviceClient.DeleteAsync(AccountConstant.LogicalName, id);
        }

        public async Task<EntityCollection> RetrieveAccountAsync(FetchExpression fetchXml)
        {
            return await serviceClient.RetrieveMultipleAsync(fetchXml);
        }

        public async Task<Entity> RetrieveAccountByIdAsync(Guid id, ColumnSet columnSet)
        {
            return await serviceClient.RetrieveAsync(AccountConstant.LogicalName, id, columnSet);
        }

        public async Task UpdateAccount(Entity entity)
        {
            await serviceClient.UpdateAsync(entity);
        }
    }
}
