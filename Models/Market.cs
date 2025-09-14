using System;
using System.Collections.Generic;
using NpgsqlTypes;

namespace grepapi.Models;

public partial class Market
{
    public long MarketId { get; set; }

    public string? MarketName { get; set; }

    public NpgsqlPoint? LocationGeo { get; set; }

    public string? GeoLink { get; set; }

    public string? MarketWeb { get; set; }

    public long ProfileType { get; set; }
}
