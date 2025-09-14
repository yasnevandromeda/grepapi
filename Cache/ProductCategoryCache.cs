using grepapi.Models;
using Microsoft.Extensions.Caching.Memory;

namespace grepapi.Cache
{
    public class ProductCategoryCache
    {
        public ProductCategoryCache() { }

        public List<ProductCategory> ProductCategoryList { get; set; } = new List<ProductCategory>();

        public Dictionary<long, ProductCategory> ProductCategoryDict { get; set; } = new Dictionary<long, ProductCategory>();
    }

    public class ProductCategoryCacheManager
    {
        public const string ProductCategoryCacheKey = "ProductCategoryCacheKey";

        public static ProductCategoryCache GetProductCategoryCache(IMemoryCache memoryCache, GrepContext grepContext)
        {
            if (!memoryCache.TryGetValue(ProductCategoryCacheKey, out ProductCategoryCache res))
            {
                res = new ProductCategoryCache();

                res.ProductCategoryList = grepContext.ProductCategories.ToList();
                res.ProductCategoryDict = res.ProductCategoryList.ToDictionary(m => m.CategoryId);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                   .SetSlidingExpiration(TimeSpan.FromHours(CacheManager.HoursInCache));

                memoryCache.Set(ProductCategoryCacheKey, res, cacheEntryOptions);
            }
            return res;
        }

    }


}
