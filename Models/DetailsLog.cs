using System;
using System.Collections.Generic;

namespace grepapi.Models;

public partial class DetailsLog
{
    public long Id { get; set; }

    public Guid? ProductId { get; set; }

    public long? MarketId { get; set; }

    public DateTime? Ts { get; set; }

    public long ShopId { get; set; }
}
