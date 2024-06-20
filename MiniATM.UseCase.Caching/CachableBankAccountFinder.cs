using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using MiniATM.Entities;
using MiniATM.UseCase.Exceptions;
using System.Text.Json;

namespace MiniATM.UseCase.Caching
{
    public class CachableBankAccountFinder : IBankAccountFinder // a cachable finder using 'cache aside' strategy
    {
        private static readonly JsonSerializerOptions serializerOptions = new() { 
            
        }; 

        private readonly CachableBankAccountFinderOptions options;
        private readonly IBankAccountFinder parentFinder;
        private readonly IDistributedCache cache;
        private readonly ILogger<CachableBankAccountFinder> logger;
        private readonly DistributedCacheEntryOptions cacheEntryOptions;

        public CachableBankAccountFinder(IBankAccountFinder parentFinder, IDistributedCache cache, CachableBankAccountFinderOptions options, ILogger<CachableBankAccountFinder> logger)
        {
            this.parentFinder = parentFinder ?? throw new ArgumentNullException(nameof(parentFinder));

            this.options = options;
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this.logger = logger;

            cacheEntryOptions = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = options.CacheTimeSpan
            };
        }

        public async Task<IEnumerable<BankAccount>> FindByCustomerIdAsync(Guid customerId)
        {
            var cacheKey = $"{options.CacheKey}#{customerId}";

            var cachedData = await cache.GetStringAsync(cacheKey);
            if (cachedData == null)
            {
                // cache missed!
                logger.LogInformation("Loading accounts from parent...");
                var accounts = await parentFinder.FindByCustomerIdAsync(customerId) ?? throw new AccountListNullException();
                cachedData = JsonSerializer.Serialize(accounts, serializerOptions);

                logger.LogInformation("Storing {data} to cache...", cachedData);
                await cache.SetStringAsync(cacheKey, cachedData, cacheEntryOptions);

                return accounts;
            }
            else
            {
                // cache hit!
                var accounts = JsonSerializer.Deserialize<IEnumerable<BankAccount>>(cachedData) ?? [];
                return accounts;
            }
        }
    }
}
