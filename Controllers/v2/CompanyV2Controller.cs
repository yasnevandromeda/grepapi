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
    [Route("api/v2/company")]
    [ApiController]
    public class CompanyV2Controller : ControllerBase
    {
        private readonly GrepContext _grepContext;

        private readonly IMemoryCache _memoryCache;

        public CompanyV2Controller(GrepContext context, IMemoryCache memoryCache)
        {
            _grepContext = context;
            _memoryCache = memoryCache;
        }

        [HttpPost("register-company")]
        public Company RegisterCompany(Company request, [FromHeader] string ApiKey)
        {
            if (ApiKey != CompanyController.AndromedaAdminApiKey)
            {
                return null;
            }

            if (_grepContext.Companies.Where(c => c.Inn == request.Inn).Count() > 0)
            {
                return null;
            }

            var company = new Company();
            company.Inn = request.Inn;
            company.CompanyName = request.CompanyName;
            company.CompanyType = request.CompanyType;
            company.BusinessMobile = SecureManager.EncryptString(request.BusinessMobile);
            company.BusinessEmail = SecureManager.EncryptString(request.BusinessEmail);
            company.BusinessAddress = SecureManager.EncryptString(request.BusinessAddress);
            company.CompanyPassword = CompanyController.CreatePassword(30);
            company.RegistrationTs = DateTime.Now;
            company.FriendInn = request.FriendInn;
            company.FriendPhone = SecureManager.EncryptString(request.FriendPhone);
            _grepContext.Companies.Add(company);
            _grepContext.SaveChanges();

            if (!string.IsNullOrEmpty(request.FriendInn)
                ||
                !string.IsNullOrEmpty(request.FriendPhone))
            {
                var promo = new CompanyPromoRequest();
                promo.CompanyId = company.CompanyId;
                promo.Active = 1;
                promo.Inn = request.FriendInn;
                promo.Phone = SecureManager.EncryptString(request.FriendPhone);
                promo.Ts = DateTime.Now;
                _grepContext.CompanyPromoRequests.Add(promo);
                _grepContext.SaveChanges();
            }

            var catalog = new Catalog();
            catalog.CatalogName = "Базовый каталог";
            catalog.CompanyId = company.CompanyId;
            _grepContext.Catalogs.Add(catalog);
            _grepContext.SaveChanges();

            return company;
        }

        [HttpPost("update-company")]
        public void UpdateCompany(Company request, [FromHeader] string Password)
        {
            CompanyController.CheckAccess(request.CompanyId, Password, _grepContext);

            var company = _grepContext.Companies.FirstOrDefault(c => c.CompanyId == request.CompanyId);
            if (company == null)
            {
                return;
            }
            company.BusinessMobile = SecureManager.EncryptString(request.BusinessMobile);
            company.BusinessEmail = SecureManager.EncryptString(request.BusinessEmail);
            company.BusinessAddress = SecureManager.EncryptString(request.BusinessAddress);
            _grepContext.Companies.Update(company);
            _grepContext.SaveChanges();
        }

    }
}
