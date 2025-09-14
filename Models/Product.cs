using System;
using System.Collections.Generic;

namespace grepapi.Models;

public partial class Product
{
    public Guid ProductId { get; set; }

    public long ShopId { get; set; }

    public string ProductName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? Picture { get; set; }

    public long Price { get; set; }

    public long Available { get; set; }

    public long CatalogId { get; set; }

    public string? SerialNo { get; set; }

    public string? PictureExt { get; set; }

    public long MarketId { get; set; }

    public string ShopName { get; set; } = "";

    public string? ManufacturerUrl { get; set; }

    public long? CategoryId { get; set; }

    public string? Brand { get; set; }

    public int PictureWidth { get; set; }

    public int PictureHeight { get; set; }

    public string? HighResPicture { get; set; }

    public string? HighResPictureExt { get; set; }

    public int HighResWidth { get; set; }

    public int HighResHeight { get; set; }

    public long Promo { get; set; } = 0;
}
