using grepapi.Cache;

namespace grepapi.Models;

public class ProductDetails : Product
{
    public ProductDetails() : base() { }

    public ProductDetails(Product p, ProductCategoryCache categories)
    {
        ProductId = p.ProductId;
        ShopId = p.ShopId;
        ProductName = p.ProductName;
        Description = p.Description;
        Picture = p.Picture ?? "";
        Price = p.Price;
        Available = p.Available;
        CatalogId = p.CatalogId;
        SerialNo = p.SerialNo ?? "";
        PictureExt = p.PictureExt ?? "";
        PictureWidth = p.PictureWidth;
        PictureHeight = p.PictureHeight;
        MarketId = p.MarketId;
        ShopName = p.ShopName;
        ManufacturerUrl = p.ManufacturerUrl;
        CategoryId = p.CategoryId;
        Brand = p.Brand;

        HighResPicture = p.HighResPicture;
        HighResPictureExt = p.HighResPictureExt;
        HighResWidth = p.HighResWidth;
        HighResHeight = p.HighResHeight;

        if(CategoryId.HasValue && categories.ProductCategoryDict.ContainsKey(CategoryId.Value))
        {
            ProductCategory = categories.ProductCategoryDict[CategoryId.Value].CategoryName;
        }
    }

    public string LocationPic { get; set; } = "";

    public string LocationPicExt { get; set; } = "";

    public string PathComments { get; set; } = "";

    public string PathLink { get; set; } = "";

    public string CustomerMobile { get; set; } = "";

    public string ShopWeb { get; set; } = "";

    public string ShopTelegram { get; set; } = "";


    public string MarketName { get; set; } = "";

    public string GeoLink { get; set; } = "";

    public string MarketWeb { get; set; } = "";

    public string ProductCategory { get; set; } = "";

}
