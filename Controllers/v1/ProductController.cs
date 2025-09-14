using Amazon.S3.Model.Internal.MarshallTransformations;
using grepapi.Cache;
using grepapi.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;


namespace grepapi.Controllers
{
    [ApiController]
    [Route("api/product")]
    public class ProductController : ControllerBase
    {

        private readonly ILogger<ProductController> _logger;

        private readonly GrepContext _grepContext;

        private readonly IMemoryCache _memoryCache;

        private const int MaxSearchCount = 201;


        public ProductController(ILogger<ProductController> logger,
            GrepContext context, IMemoryCache memChache)
        {
            _logger = logger;
            _grepContext = context;
            _memoryCache = memChache;
        }


        [HttpGet("search")]
        public IEnumerable<Product> Search(long marketid, string keywords, long? minprice, long? maxprice)
        {
            var items = FindProducts(marketid, keywords, minprice, maxprice); 
            return items;
        }

        [HttpGet("random")]
        public IEnumerable<Product> Random(long marketid)
        {
            var items = RandomProducts(marketid);
            return items;
        }


        [HttpGet("shop")]
        public Shop GetShop([FromHeader] long CompanyID, [FromHeader] string Password)
        {
            CompanyController.CheckAccess(CompanyID, Password, _grepContext);

            var shop = _grepContext.Shops.FirstOrDefault(s => s.CompanyId == CompanyID);

            return shop;
        }

        [HttpGet("product-category-list")]
        public List<ProductCategory> GetProductCategories()
        {
            var cache = ProductCategoryCacheManager.GetProductCategoryCache(_memoryCache, _grepContext);
            return cache.ProductCategoryList;
        }
        


        [HttpGet("catalog")]
        public IEnumerable<Product> GetCatalog([FromHeader] long CompanyID, [FromHeader] string Password, long catalogId)
        {
            CompanyController.CheckAccess(CompanyID, Password, _grepContext);

            var products = _grepContext.Products.Where(p => p.CatalogId == catalogId).ToList();

            return products;
        }

        [HttpGet("all-by-shop")]
        public IEnumerable<Product> GetShopProducts(long shopId)
        {
            var res = new List<Product>();
            var shopCache = ShopCacheManager.GetShopCache(_memoryCache, _grepContext);
            if (!shopCache.ShopDict.TryGetValue(shopId, out var shop))
                return res;

            var pm = ProductCacheManager.GetProductCache(_memoryCache, _grepContext);
            var catalogId = shop.CatalogId.Value;
            var allProducts = pm.Products[catalogId].ProductList;
            foreach(var p in allProducts)
            {
                p.ShopId = shopId;
                p.ShopName = shop.ShopName;
            }
            return allProducts;
        }

        [HttpPost("create")]
        public void CreateProduct(Product product, [FromHeader] long CompanyID,  [FromHeader] string Password)
        {
            CompanyController.CheckAccess(CompanyID, Password, _grepContext);

            _grepContext.Products.Add(product);
            _grepContext.SaveChanges();

            ProductCacheManager.RefreshProduct(product.CatalogId, product.ProductId,
                _memoryCache, _grepContext);
        }

        [HttpPost("create-products-admin")]
        [RequestSizeLimit(50000000)]
        public void CreateProductAdmin(Product[] products, [FromHeader] string ApiKey)
        {
            if (ApiKey != CompanyController.AndromedaAdminApiKey)
                return;

            if (products.Length == 0)
                return;

            foreach (var product in products)
            {
                _grepContext.Products.Add(product);
                _grepContext.SaveChanges();

                ProductCacheManager.RefreshProduct(product.CatalogId, product.ProductId,
                    _memoryCache, _grepContext);
            }
        }


        [HttpPost("update")]
        public void UpdateProduct(Product product, [FromHeader] long CompanyID,  [FromHeader] string Password)
        {
            CompanyController.CheckAccess(CompanyID, Password, _grepContext);

            var currentProduct = _grepContext.Products.Where(p => p.ProductId == product.ProductId).FirstOrDefault();

            if (currentProduct != null)
            {
                currentProduct.ProductName = product.ProductName;
                currentProduct.Description = product.Description;
                currentProduct.Picture = product.Picture;
                currentProduct.PictureExt = product.PictureExt;
                currentProduct.PictureWidth = product.PictureWidth;
                currentProduct.PictureHeight = product.PictureHeight;
                currentProduct.Price = product.Price;
                currentProduct.Available = product.Available;
                currentProduct.SerialNo = product.SerialNo;
                currentProduct.ShopName = "";
                currentProduct.ManufacturerUrl = product.ManufacturerUrl;
                currentProduct.CategoryId = product.CategoryId;
                currentProduct.Brand = product.Brand;
                currentProduct.HighResPicture = product.HighResPicture;
                currentProduct.HighResPictureExt = product.HighResPictureExt;
                currentProduct.HighResWidth = product.HighResWidth;
                currentProduct.HighResHeight = product.HighResHeight;
                _grepContext.Products.Update(currentProduct);
                _grepContext.SaveChanges();

                ProductCacheManager.RefreshProduct(currentProduct.CatalogId, currentProduct.ProductId,
                                        _memoryCache, _grepContext);
            }
        }

        [HttpPost("delete")]
        public void DeleteProduct(Guid productID, [FromHeader] long CompanyID, [FromHeader] string Password)
        {
            CompanyController.CheckAccess(CompanyID, Password, _grepContext);

            var currentProduct = _grepContext.Products.Where(p => p.ProductId == productID).FirstOrDefault();

            if (currentProduct == null)
                return;
            
            var marketId = currentProduct.MarketId;
            _grepContext.Products.Remove(currentProduct);
            _grepContext.SaveChanges();

            ProductCacheManager.DeleteProduct(currentProduct.CatalogId, productID, _memoryCache, _grepContext);
        }

        [HttpGet("details")]
        public ProductDetails GetProductDetails(long shopId, Guid productID)
        {
            var res = new ProductDetails();

            var shops = ShopCacheManager.GetShopCache(_memoryCache, _grepContext);

            var pm = ProductCacheManager.GetProductCache(_memoryCache, _grepContext);

            if (!shops.ShopDict.TryGetValue(shopId, out var shop))
                return res;

            var catalog = pm.Products[shop.CatalogId.Value];

            if (!catalog.ProductDict.ContainsKey(productID))
                return res;

            Product p = catalog.ProductDict[productID];
            if (p == null)
                return res;

            ProductCategoryCache categories = ProductCategoryCacheManager.GetProductCategoryCache(_memoryCache, _grepContext);

            res = new ProductDetails(p, categories);

            if (shops.ShopDict.ContainsKey(shopId))
            {
                res.LocationPic = shop.LocationPic ?? "";
                res.LocationPicExt = shop.LocationPicExt ?? "";
                res.CustomerMobile = shop.CustomerMobile ?? "";
                res.ShopWeb = shop.ShopWeb ?? "";
                res.ShopTelegram = shop.Telegram ?? "";
                res.PathLink = shop.PathLink ?? "";
                res.PathComments = shop.PathComments ?? "";

                res.ShopName = shop.ShopName;

                var markets = MarketCacheManager.GetMarketCache(_memoryCache, _grepContext);
                if (markets.MarketDict.ContainsKey(shop.MarketId.Value))
                {
                    var market = markets.MarketDict[shop.MarketId.Value];
                    res.MarketName = market.MarketName ?? "";
                    res.GeoLink = market.GeoLink ?? "";
                    res.MarketWeb = market.MarketWeb ?? "";
                }

                var details = new DetailsLog()
                { 
                    MarketId = shop.MarketId.Value,
                    ProductId = productID, 
                    Ts = DateTime.Now,
                    ShopId = shop.Id
                };
                _grepContext.DetailsLogs.Add(details);
                _grepContext.SaveChanges();
            }

            return res;
        }

        private bool ProductValidated(Product p, Shop shop, 
            ProductCategoryCache categories, string[] words, long? minprice, long? maxprice)
        {
            bool validated = false;
            int wordValidated = 0;
            var category = "";
            if (p.CategoryId.HasValue && categories.ProductCategoryDict.ContainsKey(p.CategoryId.Value))
            {
                category = categories.ProductCategoryDict[p.CategoryId.Value].CategoryName;
            }
            foreach (var word in words)
            {
                var wl = word.ToLower();
                var shopName = shop.ShopName;
                if (p.ProductName.ToLower().Contains(wl)
                    ||
                    p.Description.ToLower().Contains(wl)
                    ||
                    (p.Brand != null && p.Brand.ToLower().Contains(wl))
                    ||
                    (category != "" && category.ToLower().Contains(wl))
                    ||
                    (shopName != null && shopName.ToLower().Contains(wl))
                    )
                {
                    wordValidated++;
                    continue;
                }
                else
                {
                    break;
                }
            }
            validated = wordValidated == words.Length;
            if (!validated)
                return false;
            if (minprice.HasValue && p.Price < minprice.Value)
                return false;
            if (maxprice.HasValue && p.Price > maxprice.Value)
                return false;
            return true; 
        }

        public static void Shuffle<T>(T[] array)
        {
            Random random = new Random();
            int n = array.Length;

            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
        }

        private List<Product> RandomProducts(long marketid)
        {
            List<Product> res = new List<Product>();

            var shops = ShopCacheManager.GetShopCache(_memoryCache, _grepContext);

            var pm = ProductCacheManager.GetProductCache(_memoryCache, _grepContext);

            var categories = ProductCategoryCacheManager.GetProductCategoryCache(_memoryCache, _grepContext);

            if (!pm.MarketDict.ContainsKey(marketid))
                return res;

            var mi = pm.MarketDict[marketid];

            var randomShops = mi.Shops.ToArray();
            Shuffle(randomShops);

            var allProducts = new List<Product>();

            HashSet<Guid> resIds = new HashSet<Guid>();

            foreach (var shopId in randomShops)
            {
                var shop = shops.ShopDict[shopId];
                var catalogId = shop.CatalogId.Value;

                if (!pm.Products.ContainsKey(catalogId))
                    continue;

                foreach(var p in pm.Products[catalogId].ProductList)
                {
                    if(p.Promo == 1)
                    {
                        var promoP = new Product()
                        {
                            ProductId = p.ProductId,
                            ShopId = shopId,
                            ShopName = shop.ShopName,
                            ProductName = p.ProductName,
                            Price = p.Price,
                            Picture = p.Picture,
                            PictureExt = p.PictureExt,
                            PictureHeight = p.PictureHeight,
                            PictureWidth = p.PictureWidth
                        };
                        allProducts.Add(promoP);
                    }                    
                }
            }

            if (allProducts.Count == 0)
                return res;

            if (allProducts.Count <= 8)
                return allProducts;

            int cnt = 0;
            while(res.Count < 8)
            {
                Shuffle(randomShops);

                foreach(var s in randomShops)
                {
                    var products = allProducts.Where(p => p.ShopId == s
                                    && !resIds.Contains(p.ProductId)).ToList();

                    if (products.Count > 0)
                    {
                        int index = (new Random()).Next(products.Count - 1);
                        var p = products[index];
                        if (!resIds.Contains(p.ProductId))
                        {
                            res.Add(p);
                            resIds.Add(p.ProductId);
                        }
                    }

                    cnt++;
                }
                
                if (cnt == 2000)
                    break;
            }

            return res;
        }

        private List<Product> FindProducts(long marketid, string keywords, long? minprice, long? maxprice)
        {
            List<Product> res = new List<Product>();

            var shops = ShopCacheManager.GetShopCache(_memoryCache, _grepContext);

            var pm = ProductCacheManager.GetProductCache(_memoryCache, _grepContext);

            var categories = ProductCategoryCacheManager.GetProductCategoryCache(_memoryCache, _grepContext);

            string[] words = keywords.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            if (!pm.MarketDict.ContainsKey(marketid))
                return res;

            var mi = pm.MarketDict[marketid];

            HashSet<Guid> resIds = new HashSet<Guid>();

            var randomShops = mi.Shops.ToArray();
            Shuffle(randomShops);

            foreach(var shopId in randomShops)
            {
                var shop = shops.ShopDict[shopId];
                var catalogId = shop.CatalogId.Value;

                if (!pm.Products.ContainsKey(catalogId))
                    continue;

                var allProducts = pm.Products[catalogId].ProductList;

                foreach(var p in allProducts)
                {
                    if (ProductValidated(p, shop, categories, words, minprice, maxprice)
                        && !resIds.Contains(p.ProductId))
                    {
                        p.ShopId = shop.Id;
                        p.ShopName = shop.ShopName;
                        res.Add(p);
                        resIds.Add(p.ProductId);
                    }

                    if (res.Count == MaxSearchCount)
                    {
                        break;
                    }
                }                
            }

            SearchLog log = new SearchLog()
             {
                 MarketId = marketid,
                 Keywords = keywords.Truncate(495),
                 PriceMin = minprice,
                 PriceMax = maxprice,
                 ProductFound = res.Count,
                 Ts = DateTime.Now
             };
             _grepContext.SearchLogs.Add(log);
             _grepContext.SaveChanges();

            return res;
        }

        

    }
}
