using System;
using System.Collections.Generic;

namespace grepapi.Models;

public partial class SearchLog
{
    public long Id { get; set; }

    public long? MarketId { get; set; }

    public string? Keywords { get; set; }

    public int? ProductFound { get; set; }

    public long? PriceMin { get; set; }

    public long? PriceMax { get; set; }

    public DateTime? Ts { get; set; }
}
