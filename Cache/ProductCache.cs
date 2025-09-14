using grepapi.Models;
using Microsoft.Extensions.Caching.Memory;

namespace grepapi.Cache
{
    public class ProductByMarketCache
    {
        public ProductByMarketCache() { }

        /// <summary>
        /// кэш продуктов по каталогам
        /// </summary>
        public Dictionary<long, ProductCache> Products { get; set; } = new Dictionary<long, ProductCache>();

        /// <summary>
        /// кэш описаний рынков
        /// </summary>
        public Dictionary<long, MarketInfo> MarketDict { get; set; } = new Dictionary<long, MarketInfo>();
    }

    public class MarketInfo
    {
        public MarketInfo() { }

        public long MarketId { get; set; }

        public List<long> Shops { get; set; } = new List<long>();
    }

    public class ProductCache
    {
        public ProductCache()
        {
        }

        public long CatalogId { get; set; }

        public List<Product> ProductList { get; set; } = new List<Product>();

        public Dictionary<Guid, Product> ProductDict { get; set; } = new Dictionary<Guid, Product>();
    }

    public class ProductCacheManager
    {
        public const string ProductCacheKey = "ProductCacheKey";

        public static ProductByMarketCache GetProductCache(IMemoryCache memoryCache, GrepContext grepContext)
        {
            if (!memoryCache.TryGetValue(ProductCacheKey, out ProductByMarketCache res))
            {
                res = new ProductByMarketCache();

                // магазины
                var shops = ShopCacheManager.GetShopCache(memoryCache, grepContext);

                foreach(var s in shops.ShopList)
                {
                    MarketInfo mi;
                    if(!res.MarketDict.ContainsKey(s.MarketId.Value))
                    {
                        mi = new MarketInfo() { MarketId = s.MarketId.Value };
                        res.MarketDict[s.MarketId.Value] = mi;
                    }
                    else
                    {
                        mi = res.MarketDict[s.MarketId.Value];
                    }
                    mi.Shops.Add(s.Id);
                }

                // продукты
                var products = grepContext.Products.ToList();

                foreach (var p in products)
                {
                    if (!res.Products.TryGetValue(p.CatalogId, out ProductCache catalog))
                    {
                        catalog = new ProductCache()
                        {
                            CatalogId = p.CatalogId,
                            ProductList = new List<Product>(),
                            ProductDict = new Dictionary<Guid, Product>()
                        };
                        res.Products.Add(p.CatalogId, catalog);
                    }

                    catalog.ProductList.Add(p);
                    catalog.ProductDict.Add(p.ProductId, p);
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                   .SetSlidingExpiration(TimeSpan.FromHours(CacheManager.HoursInCache));

                memoryCache.Set(ProductCacheKey, res, cacheEntryOptions);
            }
            return res;
        }

        public static void RefreshProduct(long catalogId, Guid productId, IMemoryCache memoryCache, GrepContext grepContext)
        {
            var productByMarketCache = ProductCacheManager.GetProductCache(memoryCache, grepContext);
            if (productByMarketCache == null)
                return;

            var product = grepContext.Products.FirstOrDefault(p => p.ProductId == productId);

            if (productByMarketCache.Products.TryGetValue(catalogId, out ProductCache catalog))
            {
                var p = catalog.ProductList.FirstOrDefault(p => p.ProductId == productId);
                if (p != null)
                    catalog.ProductList.Remove(p);
                if (catalog.ProductDict.ContainsKey(productId))
                    catalog.ProductDict.Remove(productId);

                catalog.ProductList.Add(product);
                catalog.ProductDict.Add(productId, product);
            }
        }

        public static void DeleteProduct(long catalogId, Guid productId, IMemoryCache memoryCache, GrepContext grepContext)
        {
            var productByMarketCache = ProductCacheManager.GetProductCache(memoryCache, grepContext);
            if (productByMarketCache == null)
                return;

            if (productByMarketCache.Products.TryGetValue(catalogId, out ProductCache catalog))
            {
                var p = catalog.ProductList.FirstOrDefault(p => p.ProductId == productId);
                if (p != null)
                    catalog.ProductList.Remove(p);
                if (catalog.ProductDict.ContainsKey(productId))
                    catalog.ProductDict.Remove(productId);
            }
        }

    }
}
