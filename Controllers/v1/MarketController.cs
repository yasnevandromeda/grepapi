using grepapi.Cache;
using grepapi.Models;
using grepapi.SearchModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;

namespace grepapi.Controllers
{
    [ApiController]
    [Route("api/market")]
    public class MarketController : ControllerBase
    {

        private readonly ILogger<ProductController> _logger;

        private readonly GrepContext _grepContext;

        private readonly IMemoryCache _memoryCache;

        public MarketController(ILogger<ProductController> logger,
            GrepContext context, IMemoryCache memChache)
        {
            _logger = logger;
            _grepContext = context;
            _memoryCache = memChache;
        }


        [HttpGet("list")]
        public IEnumerable<MarketBrief> GetList()
        {
            var mc = MarketCacheManager.GetMarketCache(_memoryCache, _grepContext);

            var res = mc.MarketList.Select(m => new MarketBrief(m)).OrderBy(m => m.MarketName);

            return res.ToList();
        }

        [HttpPost("clear-cache")]
        public async Task ClearAllCache()
        {
            _memoryCache.Remove(MarketCacheManager.MarketCacheKey);
            _memoryCache.Remove(ProductCacheManager.ProductCacheKey);
            _memoryCache.Remove(ProductCategoryCacheManager.ProductCategoryCacheKey);
            _memoryCache.Remove(ShopCacheManager.ShopCacheKey);
        }

    }
}
