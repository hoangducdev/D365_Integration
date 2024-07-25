using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Integration.AzureFunctionApp.Interfaces
{
    public interface ITokenCache
    {
        void AddToCache(string tokenCacheKey, string token, int expires_in);
        string GetFromCache(string tokenCacheKey);
    }
}
