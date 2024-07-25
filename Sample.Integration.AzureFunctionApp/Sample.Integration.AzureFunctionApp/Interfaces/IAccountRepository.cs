using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Threading.Tasks;

namespace Sample.Integration.AzureFunctionApp.Interfaces
{
    public interface IAccountRepository
    {
        Task<Guid> CreateAccount(Entity entity);
        Task<EntityCollection> RetrieveAccountAsync(FetchExpression fetchXml);
        Task<Entity> RetrieveAccountByIdAsync(Guid id, ColumnSet columnSet);
        Task DeleteAccount(Guid id);
        Task UpdateAccount(Entity entity);
    }
}
