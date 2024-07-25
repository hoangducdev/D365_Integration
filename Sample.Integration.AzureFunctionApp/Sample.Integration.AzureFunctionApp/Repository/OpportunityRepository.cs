using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Sample.Integration.AzureFunctionApp.Constant;
using Sample.Integration.AzureFunctionApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Sample.Integration.AzureFunctionApp.Repository
{
    public class OpportunityRepository : IOpportunityRepository
    {
        private readonly IConnectionService _connectionService;
        private readonly ServiceClient serviceClient;
        private readonly ILogger<OpportunityRepository> _logger;
        private readonly ParallelOptions _parallelOptions;

        public OpportunityRepository(IConnectionService connectionService, ILogger<OpportunityRepository> logger) 
        {
            _connectionService = connectionService;
            serviceClient = _connectionService.CreateServiceClient();
            _logger = logger;
            _parallelOptions = new()
            {
                MaxDegreeOfParallelism = serviceClient.RecommendedDegreesOfParallelism
            };
        }

        public async Task<Guid> CreateOpportunityAsync(Entity entity)
        {
            return await serviceClient.CreateAsync(entity);
        }

        public async Task DeleteOpportunityAsync(Guid id)
        {
            await serviceClient.DeleteAsync(OpportunityConstant.LogicalName, id);
        }

        public async Task<EntityCollection> RetrieveOpportunityAsync(FetchExpression fetchXml)
        {
            return await serviceClient.RetrieveMultipleAsync(fetchXml);
        }

        public async Task UpdateMultipleOpportunityAsync(List<Entity> recordsToUpdate)
        {
            try
            {
                UpdateMultipleResponse response = new UpdateMultipleResponse();
                await Parallel.ForEachAsync(
                    source: recordsToUpdate.Chunk(1000),
                    parallelOptions: _parallelOptions,
                    async (entities, token) =>
                    {
                        UpdateMultipleRequest updateMultipleRequest = new()
                        {
                            Targets = new EntityCollection(recordsToUpdate)
                            {
                                EntityName = OpportunityConstant.LogicalName
                            },
                        };
                        response = (UpdateMultipleResponse)await serviceClient.ExecuteAsync(
                             request: updateMultipleRequest,
                             cancellationToken: token);
                    });
            } 
            catch (Exception ex)
            {
                _logger.LogInformation("Fail in UpdateMultipleRequest!");
                throw;
            } 
        }

        public async Task ExcuteMultipleOpportunityAsync(List<OrganizationRequest> requests, bool continueOrError = true)
        {
            ExecuteMultipleRequest executeMultipleRequest = new()
            {
                Requests = new OrganizationRequestCollection(),
                Settings = new ExecuteMultipleSettings()
                {
                    ReturnResponses = true,
                    ContinueOnError = continueOrError
                }
            };
            var count = requests.Count > 500 ? requests.Count / 500 : requests.Count != 0 ? 1 : 0;
            try
            {
                for (int i = 0; i < count; i++)
                {
                    var pagination = from p in requests.Skip(i * 500).Take(500)
                                     select p;
                    foreach (var request in pagination)
                    {
                        executeMultipleRequest.Requests.Add(request);
                    }
                    var responses = await serviceClient.ExecuteAsync(executeMultipleRequest);
                    var response = (ExecuteMultipleResponse)responses;
                    foreach (var r in response.Responses)
                    {
                        if (r.Response != null)
                        {
                            _logger.LogInformation("Record Opportunity Updated");
                        }
                        else if (r.Fault != null)
                            _logger.LogInformation(r.Fault.Message);
                    }
                    executeMultipleRequest.Requests.Clear();
                }
            }
            catch (FaultException<OrganizationServiceFault> fault)
            {
                // Check if the maximum batch size has been exceeded. The maximum batch size is only included in the fault if
                // the input request collection count exceeds the maximum batch size.
                if (fault.Detail.ErrorDetails.Contains("MaxBatchSize"))
                {
                    int maxBatchSize = Convert.ToInt32(fault.Detail.ErrorDetails["MaxBatchSize"]);
                    if (maxBatchSize < executeMultipleRequest.Requests.Count)
                    {
                        // Here you could reduce the size of your request collection and re-submit the ExecuteMultiple request.
                        // For this sample, that only issues a few requests per batch, we will just print out some info. However,
                        // this code will never be executed because the default max batch size is 1000.
                        Console.WriteLine("The input request collection contains %0 requests, which exceeds the maximum allowed (%1)",
                            executeMultipleRequest.Requests.Count, maxBatchSize);
                    }
                }
                // Re-throw so Main() can process the fault.
                throw;
            }
        }

        public async Task UpdateOpportunityAsync(Entity entity)
        {
            await serviceClient.UpdateAsync(entity);
        }

        public async Task<EntityCollection> ProcessMultipleRequestAsync(List<OrganizationRequest> requests, bool continueOrError = true)
        {
            ExecuteMultipleRequest executeMultipleRequest = new ExecuteMultipleRequest()
            {
                Requests = new OrganizationRequestCollection(),
                Settings = new ExecuteMultipleSettings()
                {
                    ReturnResponses = true,
                    ContinueOnError = continueOrError
                }
            };

            EntityCollection processedEntities = new EntityCollection();

            var count = requests.Count > 500 ? requests.Count / 500 : requests.Count != 0 ? 1 : 0;

            for (int i = 0; i < count; i++)
            {
                var pagination = requests.Skip(i * 500).Take(500);

                foreach (var request in pagination)
                {
                    executeMultipleRequest.Requests.Add(request);
                }

                var responses = await serviceClient.ExecuteAsync(executeMultipleRequest);
                var response = (ExecuteMultipleResponse)responses;

                foreach (var r in response.Responses)
                {
                    if (r.Response != null)
                    {
                        if (r.Response is RetrieveMultipleResponse)
                        {
                            var retrieveMultipleResponse = (RetrieveMultipleResponse)r.Response;
                            var entityCollection = (EntityCollection)retrieveMultipleResponse.EntityCollection;
                            processedEntities.Entities.AddRange(entityCollection.Entities);
                        }
                        else if (r.Response is CreateResponse || r.Response is UpdateResponse)
                        {

                            _logger.LogInformation("Record " + r.Response.ResponseName + "d");
                        }
                    }
                    else if (r.Fault != null)
                    {
                        _logger.LogInformation(r.Fault.Message);
                    }
                }

                executeMultipleRequest.Requests.Clear();
            }

            return processedEntities;
        }

        public async Task<EntityCollection> RetrieveMultipleOpportunityAsync(List<string> fetchXmlToRetrieve)
        {
            try
            {
                EntityCollection combinedEntityCollection = new EntityCollection();
                await Parallel.ForEachAsync(
                    source: fetchXmlToRetrieve.Chunk(1000),
                    parallelOptions: _parallelOptions,
                    async (fetchXmlChunk, token) =>
                    {
                        foreach (var fetchXml in fetchXmlChunk)
                        {
                            RetrieveMultipleRequest retrieveMultipleRequest = new RetrieveMultipleRequest
                            {
                                Query = new FetchExpression(fetchXml)
                            };
                            retrieveMultipleRequest["BypassCustomPluginExecution"] = true;
                            RetrieveMultipleResponse retrieveMultipleResponse = (RetrieveMultipleResponse)await serviceClient.ExecuteAsync(
                                request: retrieveMultipleRequest,
                                cancellationToken: token);
                            // Combine results
                            lock (combinedEntityCollection)
                            {
                                combinedEntityCollection.Entities.AddRange(retrieveMultipleResponse.EntityCollection.Entities);
                            }
                        }
                    });
                return combinedEntityCollection;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                throw;
            }
        }

        public async Task<EntityCollection> RetrieveMultipleAsync(string fetchXml)
        {
            EntityCollection fetchResult = new();
            string pagingCookie = null;
            int pageNumber = 1;

            while (true)
            {
                EntityCollection currentRecords = await serviceClient.RetrieveMultipleAsync(new FetchExpression(fetchXml));

                if (currentRecords.Entities.Count != 0)
                {
                    fetchResult.Entities.AddRange(currentRecords.Entities);
                    if (currentRecords.MoreRecords)
                    {
                        fetchXml = Helper.Helper.CreateXml(fetchXml, pagingCookie, ++pageNumber);
                    }
                    else break;
                }
                else break;
            }
            return fetchResult;
        }
        
        public async Task<Entity> RetrieveOpportunityById(Guid id, ColumnSet columnSet)
        {
            return await serviceClient.RetrieveAsync(OpportunityConstant.LogicalName, id, columnSet);
        }
        public async Task<EntityCollection> HandleRetrieveMore5000RecordAsync(string fetchXml)
        {
            EntityCollection fetchResult = new();
            string pagingCookie = null;
            int pageNumber = 1;

            while (true)
            {
                EntityCollection currentRecords = await serviceClient.RetrieveMultipleAsync(new FetchExpression(fetchXml));

                if (currentRecords.Entities.Count != 0)
                {
                    fetchResult.Entities.AddRange(currentRecords.Entities);
                    if (currentRecords.MoreRecords)
                    {
                        fetchXml = Helper.Helper.CreateXml(fetchXml, pagingCookie, ++pageNumber);
                    }
                    else break;
                }
                else break;
            }
            return fetchResult;
        }
    }
}
