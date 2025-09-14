using grepapi.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Caching.Memory;

namespace grepapi.Cache
{
    public class ShopCache
    {
        public ShopCache() { }

        public HashSet<long> ActiveShops { get; set; } = new HashSet<long> {  };

        public List<Shop> ShopList { get; set; } = new List<Shop>();

        public Dictionary<long, Shop> ShopDict { get; set; } = new Dictionary<long, Shop>();
    }

    public class ShopCacheManager
    {
        public const string ShopCacheKey = "ShopCacheKey";

        public static ShopCache GetShopCache(IMemoryCache memoryCache, GrepContext grepContext)
        {
            if (!memoryCache.TryGetValue(ShopCacheKey, out ShopCache res))
            {
                res = new ShopCache();

                res.ShopList = grepContext.Shops.Where(s => s.Active == 1).ToList();
                res.ShopDict = res.ShopList.ToDictionary(s => s.Id);

                res.ActiveShops = new HashSet<long>();
                foreach(var s in res.ShopList)
                {
                    res.ActiveShops.Add(s.Id);
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                   .SetSlidingExpiration(TimeSpan.FromHours(CacheManager.HoursInCache));

                memoryCache.Set(ShopCacheKey, res, cacheEntryOptions);
            }
            return res;
        }

        public static void RefreshShop(long shopId, IMemoryCache memoryCache, GrepContext grepContext)
        {
            ShopCache cache = GetShopCache(memoryCache, grepContext);

            if(cache != null)
            {
                var s = cache.ShopList.FirstOrDefault(s => s.Id == shopId);
                cache.ActiveShops.Remove(shopId);
                if (s != null)
                    cache.ShopList.Remove(s);
                if(cache.ShopDict.ContainsKey(shopId))
                    cache.ShopDict.Remove(shopId);

                var shop = grepContext.Shops.FirstOrDefault(s => s.Id == shopId && s.Active == 1);
                if (shop != null)
                {
                    cache.ActiveShops.Add(shopId);
                    cache.ShopList.Add(shop);
                    cache.ShopDict.Add(shopId, shop);
                }
            }
            
        }

    }

}
