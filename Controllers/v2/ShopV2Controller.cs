using Amazon.Runtime.Internal;
using grepapi.Cache;
using grepapi.Models;
using grepapi.SearchModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.Design;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace grepapi.Controllers
{
    [Route("api/v2/shop")]
    [ApiController]
    public class ShopV2Controller : ControllerBase
    {
        private readonly GrepContext _grepContext;

        private readonly IMemoryCache _memoryCache;

        public ShopV2Controller(GrepContext context, IMemoryCache memoryCache)
        {
            _grepContext = context;
            _memoryCache = memoryCache;
        }
   
        [HttpGet("get-shops")]
        public List<Shop> GetCompanyShops([FromHeader] long CompanyId, [FromHeader] string Password)
        {
            CompanyController.CheckAccess(CompanyId, Password, _grepContext);

            var shops = _grepContext.Shops.Where(s => s.CompanyId == CompanyId).ToList();
            return shops;
        }

        [HttpGet("get-catalogs")]
        public List<Catalog> GetCompanyCatalogs([FromHeader] long CompanyId, [FromHeader] string Password)
        {
            CompanyController.CheckAccess(CompanyId, Password, _grepContext);

            var catalogs = _grepContext.Catalogs.Where(s => s.CompanyId == CompanyId).ToList();
            return catalogs;
        }

        [HttpPost("create-catalog")]
        public Catalog CreateCatalog(Catalog newCatalog, [FromHeader] long CompanyId, [FromHeader] string Password)
        {
            CompanyController.CheckAccess(CompanyId, Password, _grepContext);

            var catalog = new Catalog();
            catalog.CatalogName = newCatalog.CatalogName;
            catalog.CompanyId = CompanyId;
            _grepContext.Catalogs.Add(catalog);
            _grepContext.SaveChanges();

            return catalog;
        }

        [HttpPost("create")]
        public Shop CreateShop(Shop request, [FromHeader] long CompanyId, [FromHeader] string Password)
        {
            CompanyController.CheckAccess(CompanyId, Password, _grepContext);

            var shop = new Shop();
            shop.ShopName = request.ShopName;
            shop.LocationPic = request.LocationPic;
            shop.LocationPicExt = request.LocationPicExt;
            shop.Active = 1;
            shop.ShopType = request.ShopType;
            shop.CustomerMobile = request.CustomerMobile;
            shop.MarketId = request.MarketId;
            shop.ShopWeb = request.ShopWeb;
            shop.CompanyId = CompanyId;
            shop.CatalogId = request.CatalogId;
            shop.TradeSegmentId = request.TradeSegmentId;
            shop.PathComments = request.PathComments;
            shop.Telegram = request.Telegram;
            shop.PathLink = request.PathLink;
            shop.RegistrationDate = DateTime.Now;
            _grepContext.Shops.Add(shop);
            _grepContext.SaveChanges();

            return shop;
        }

        [HttpPost("update")]
        public void Update(Shop request, [FromHeader] long CompanyId, [FromHeader] string Password)
        {
            CompanyController.CheckAccess(CompanyId, Password, _grepContext);

            var shop = _grepContext.Shops.FirstOrDefault(s => s.Id == request.Id);

            if (shop == null)
                return;

            shop.ShopName = request.ShopName;
            shop.LocationPic = request.LocationPic;
            shop.LocationPicExt = request.LocationPicExt;
            shop.CustomerMobile = request.CustomerMobile;
            shop.ShopWeb = request.ShopWeb;
            shop.CatalogId = request.CatalogId;
            shop.TradeSegmentId = request.TradeSegmentId;
            shop.PathComments = request.PathComments;
            shop.Telegram = request.Telegram;
            shop.PathLink = request.PathLink;
            _grepContext.Shops.Update(shop);
            _grepContext.SaveChanges();
        }

    }
}
