using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Sample.Integration.AzureFunctionApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Sample.Integration.AzureFunctionApp.Interfaces
{
    public interface IDataverseService
    {
        Task<EntityCollection> RetrieveAccountAsync(FetchExpression fetchXml);
    }
}
