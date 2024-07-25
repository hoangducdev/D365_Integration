using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample.Integration.AzureFunctionApp.Interfaces
{
    public interface IOpportunityRepository
    {
        Task<Guid> CreateOpportunityAsync(Entity entity);
        Task<EntityCollection> RetrieveOpportunityAsync(FetchExpression fetchXml);
        Task DeleteOpportunityAsync(Guid id);
        Task UpdateOpportunityAsync(Entity entity);
        Task ExcuteMultipleOpportunityAsync(List<OrganizationRequest> request,bool continueOrError);
        Task<EntityCollection> ProcessMultipleRequestAsync(List<OrganizationRequest> requests, bool continueOrError);
        Task UpdateMultipleOpportunityAsync(List<Entity> recordsToUpdate);
        Task<EntityCollection> RetrieveMultipleOpportunityAsync(List<string> fetchXmlToRetrieve);
        Task<EntityCollection> RetrieveMultipleAsync(string fetchXml);
        Task<Entity> RetrieveOpportunityById(Guid id, ColumnSet columnSet);
    }
}
