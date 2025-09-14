using Amazon.Runtime.Internal;
using grepapi.Cache;
using grepapi.Models;
using grepapi.SearchModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.Design;
using System.Text;

namespace grepapi.Controllers
{
    [Route("api/v2/trade-segment")]
    [ApiController]
    public class TradeSegmentV2Controller : ControllerBase
    {
        private readonly GrepContext _grepContext;

        private readonly IMemoryCache _memoryCache;

        public TradeSegmentV2Controller(GrepContext context, IMemoryCache memoryCache)
        {
            _grepContext = context;
            _memoryCache = memoryCache;
        }
   
        [HttpGet("")]
        public List<TradeSegment> GetTradeSegments()
        {
            var segments = _grepContext.TradeSegments.ToList();
            return segments;
        }
    }
}
