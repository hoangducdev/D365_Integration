using Microsoft.Extensions.Caching.Memory;
using Sample.Integration.AzureFunctionApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Integration.AzureFunctionApp
{
    public class TokenCache : ITokenCache
    {
        private readonly IMemoryCache _memoryCache;

        public TokenCache(IMemoryCache memoryCache)
        {
            this._memoryCache = memoryCache;
        }

        public void AddToCache(string tokenCacheKey, string item, int expires_in)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
            {
                SlidingExpiration = TimeSpan.FromSeconds(expires_in),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expires_in)
            };

            _memoryCache.Set(tokenCacheKey, item, cacheEntryOptions);
        }

        public string GetFromCache(string tokenCacheKey)
        {
            if (_memoryCache.TryGetValue(tokenCacheKey, out string cachedValue))
            {
                return cachedValue;
            }
            return string.Empty;
        }
    }
}
