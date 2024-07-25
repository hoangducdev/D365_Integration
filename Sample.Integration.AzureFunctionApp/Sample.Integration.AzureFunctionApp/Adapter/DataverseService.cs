using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Sample.Integration.AzureFunctionApp.Constant;
using Sample.Integration.AzureFunctionApp.Interfaces;
using Sample.Integration.AzureFunctionApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Integration.AzureFunctionApp.Adapter
{
    public class DataverseService : IDataverseService
    {
        private readonly IOpportunityRepository _opportunityRepository;
        private readonly IAccountRepository _accountRepository;     
        public DataverseService(
            IOpportunityRepository opportunityRepository, IAccountRepository accountRepository)
        {
            _opportunityRepository = opportunityRepository;
            _accountRepository = accountRepository;    
        }

        public async Task<EntityCollection> RetrieveAccountAsync(FetchExpression fetchXml)
        {
            return await _accountRepository.RetrieveAccountAsync(fetchXml);
        }
    }
}
