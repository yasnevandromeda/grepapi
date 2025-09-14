using grepapi.Models;
using Microsoft.Extensions.Caching.Memory;

namespace grepapi.Cache
{
    public class MarketCache
    {
        public MarketCache() { }

        public List<Market> MarketList { get; set; } = new List<Market>();

        public Dictionary<long, Market> MarketDict { get; set; } = new Dictionary<long, Market>();
    }

    public class MarketCacheManager
    {
        public const string MarketCacheKey = "MarketCacheKey";

        public static MarketCache GetMarketCache(IMemoryCache memoryCache, GrepContext grepContext)
        {
            if (!memoryCache.TryGetValue(MarketCacheKey, out MarketCache res))
            {
                res = new MarketCache();

                res.MarketList = grepContext.Markets.ToList();
                res.MarketDict = res.MarketList.ToDictionary(m => m.MarketId);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                   .SetSlidingExpiration(TimeSpan.FromHours(CacheManager.HoursInCache));

                memoryCache.Set(MarketCacheKey, res, cacheEntryOptions);
            }
            return res;
        }

    }


}
