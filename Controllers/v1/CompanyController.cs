using grepapi.Cache;
using grepapi.Models;
using grepapi.SearchModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.Design;
using System.Text;

namespace grepapi.Controllers
{
    [Route("api/v1/company")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly GrepContext _grepContext;

        private readonly IMemoryCache _memoryCache;

        public const string AndromedaApiKey = "qf6MGgLj9yCXIwteN6b8tWXLtmPimL";

        public const string AndromedaAdminApiKey = "XOFFCDNi9HvzXBq3K16joDr4v1kfx8yPo";

        public CompanyController(GrepContext context, IMemoryCache memoryCache)
        {
            _grepContext = context;
            _memoryCache = memoryCache;
        }

        public static void CheckAccess(long companyId, string password, GrepContext context)
        {
            var company = context.Companies.FirstOrDefault(c => c.CompanyId == companyId);
            if (company == null || company.CompanyPassword != password)
            {
                throw new ApplicationException("Доступ запрещен");
            }
        }

        [HttpGet()]
        public Company GetCompany([FromHeader] long CompanyId, [FromHeader] string Password)
        {
            CheckAccess(CompanyId, Password, _grepContext);

            var company = _grepContext.Companies.FirstOrDefault(c => c.CompanyId == CompanyId);

            if(company != null)
            {
                company.BusinessMobile = SecureManager.DecryptString(company.BusinessMobile);
                company.BusinessEmail = SecureManager.DecryptString(company.BusinessEmail);
                company.BusinessAddress = SecureManager.DecryptString(company.BusinessAddress);
                company.FriendPhone = SecureManager.DecryptString(company.FriendPhone);
            }

            return company;
        }

        [HttpGet("company-info")]
        public CompanyInfo GetCompanyInfo([FromHeader] long CompanyId, [FromHeader] string Password)
        {
            CheckAccess(CompanyId, Password, _grepContext);

            var company = _grepContext.Companies.FirstOrDefault(c => c.CompanyId == CompanyId);
            if (company != null)
            {
                company.BusinessMobile = SecureManager.DecryptString(company.BusinessMobile);
                company.BusinessEmail = SecureManager.DecryptString(company.BusinessEmail);
                company.BusinessAddress = SecureManager.DecryptString(company.BusinessAddress);
                company.FriendPhone = SecureManager.DecryptString(company.FriendPhone);

                var res = new CompanyInfo();
                var shop = _grepContext.Shops.FirstOrDefault(s => s.CompanyId == CompanyId);
                res.Company = company;
                res.Shop = shop;
                return res;
            }
            return null;
        }

        [HttpGet("company-info-admin")]
        public CompanyInfo GetCompanyInfoAdmin(string companyInn, [FromHeader] string ApiKey)
        {
            if (ApiKey != AndromedaAdminApiKey)
            {
                return null;
            }

            var company = _grepContext.Companies.FirstOrDefault(c => c.Inn == companyInn);
            if (company != null)
            {
                company.BusinessMobile = SecureManager.DecryptString(company.BusinessMobile);
                company.BusinessEmail = SecureManager.DecryptString(company.BusinessEmail);
                company.BusinessAddress = SecureManager.DecryptString(company.BusinessAddress);
                company.FriendPhone = SecureManager.DecryptString(company.FriendPhone);

                company.CompanyPassword = "";

                var res = new CompanyInfo();
                var shop = _grepContext.Shops.FirstOrDefault(s => s.CompanyId == company.CompanyId);
                res.Company = company;
                res.Shop = shop;
                return res;
            }
            return null;
        }

        [HttpPost("register-company")]
        public Company RegisterCompany(CompanyInfo request, [FromHeader] string ApiKey)
        {
            if(ApiKey != AndromedaApiKey)
            {
                return null;
            }

            if(_grepContext.Companies.Where(c => c.Inn == request.Company.Inn).Count() > 0)
            {
                return null;
            }

            var company = new Company();
            company.Inn = request.Company.Inn;
            company.CompanyName = request.Company.CompanyName;
            company.CompanyType = request.Company.CompanyType;
            company.BusinessMobile = SecureManager.EncryptString(request.Company.BusinessMobile);
            company.BusinessEmail = SecureManager.EncryptString(request.Company.BusinessEmail);
            company.BusinessAddress = SecureManager.EncryptString(request.Company.BusinessAddress);
            company.CompanyPassword = CreatePassword(30);
            company.RegistrationTs = DateTime.Now;
            company.FriendInn = request.Company.FriendInn;
            company.FriendPhone = SecureManager.EncryptString(request.Company.FriendPhone);
            _grepContext.Companies.Add(company);
            _grepContext.SaveChanges();

            if(!string.IsNullOrEmpty(request.Company.FriendInn)
                ||
                !string.IsNullOrEmpty(request.Company.FriendPhone))
            { 
                var promo = new CompanyPromoRequest();
                promo.CompanyId = company.CompanyId;
                promo.Active = 1;
                promo.Inn = request.Company.FriendInn;
                promo.Phone = SecureManager.EncryptString(request.Company.FriendPhone);
                promo.Ts = DateTime.Now;
                _grepContext.CompanyPromoRequests.Add(promo);
                _grepContext.SaveChanges();
            }

            var markets = MarketCacheManager.GetMarketCache(_memoryCache, _grepContext);
            var market = markets.MarketList.FirstOrDefault(m => m.MarketId == request.Shop.MarketId);

            var activation = new CompanyActivation();
            activation.CompanyId = company.CompanyId;
            activation.ActivationStart = DateTime.Now;
            if(market.ProfileType == 1)
                activation.ActivationEnd = activation.ActivationStart.AddDays(14);
            else
                activation.ActivationEnd = activation.ActivationStart.AddDays(45);
            activation.Start = 1;
            _grepContext.CompanyActivations.Add(activation);
            _grepContext.SaveChanges();

            var catalog = new Catalog();
            catalog.CatalogName = "Каталог " + request.Shop.ShopName;
            _grepContext.Catalogs.Add(catalog);
            _grepContext.SaveChanges();

            var shop = new Shop();
            shop.ShopName = request.Shop.ShopName;
            shop.LocationPic = request.Shop.LocationPic;
            shop.LocationPicExt = request.Shop.LocationPicExt;
            shop.Active = 1;
            shop.ShopType = 1;
            shop.CustomerMobile = request.Shop.CustomerMobile;
            shop.MarketId = request.Shop.MarketId;
            shop.ShopWeb = request.Shop.ShopWeb;
            shop.CompanyId = company.CompanyId;
            shop.CatalogId = catalog.CatalogId;
            shop.PathComments = request.Shop.PathComments;
            shop.Telegram = request.Shop.Telegram;
            shop.PathLink = request.Shop.PathLink;
            _grepContext.Shops.Add(shop);
            _grepContext.SaveChanges();

            ShopCacheManager.RefreshShop(shop.Id, _memoryCache, _grepContext);
            
            return company;
        }

        [HttpPost("update-company")]
        public void UpdateCompany(CompanyInfo request, [FromHeader] string Password)
        {
            CheckAccess(request.Company.CompanyId, Password, _grepContext);

            var company = _grepContext.Companies.FirstOrDefault(c => c.CompanyId == request.Company.CompanyId);
            if (company == null)
            {
                return;
            }
            company.BusinessMobile = SecureManager.EncryptString(request.Company.BusinessMobile);
            company.BusinessEmail = SecureManager.EncryptString(request.Company.BusinessEmail);
            company.BusinessAddress = SecureManager.EncryptString(request.Company.BusinessAddress);
            _grepContext.Companies.Update(company);
            _grepContext.SaveChanges();

            var shop = _grepContext.Shops.FirstOrDefault(s => s.Id == request.Shop.Id);

            if (shop == null)
                return;

            shop.ShopName = request.Shop.ShopName;
            shop.LocationPic = request.Shop.LocationPic;
            shop.LocationPicExt = request.Shop.LocationPicExt;
            shop.CustomerMobile = request.Shop.CustomerMobile;
            shop.ShopWeb = request.Shop.ShopWeb;
            shop.PathComments = request.Shop.PathComments;
            shop.Telegram = request.Shop.Telegram;
            shop.PathLink = request.Shop.PathLink;
            _grepContext.Shops.Update(shop);
            _grepContext.SaveChanges();

            ShopCacheManager.RefreshShop(shop.Id, _memoryCache, _grepContext);
        }

        public static string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
    }
}
