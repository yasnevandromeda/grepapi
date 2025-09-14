using System;
using System.Collections.Generic;
using NpgsqlTypes;

namespace grepapi.Models;

public partial class Shop
{
    public long Id { get; set; }

    public string? ShopName { get; set; }

    public NpgsqlPoint? LocationGeo { get; set; }

    public string? LocationPic { get; set; }

    public string? Email { get; set; }

    public string? Mobile { get; set; }

    public string? Inn { get; set; }

    public int? Active { get; set; }

    public long ShopType { get; set; }

    public string? CustomerMobile { get; set; }

    public long? MarketId { get; set; }

    public long? CatalogId { get; set; }

    public string? LocationPicExt { get; set; }

    public string? ShopWeb { get; set; }

    public long CompanyId { get; set; }

    public string? PathComments { get; set; }

    public string Telegram { get; set; } = null!;

    public string PathLink { get; set; } = null!;

    public long TradeSegmentId { get; set; }

    public DateTime RegistrationDate { get; set; }
}
