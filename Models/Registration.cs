using System;
using System.Collections.Generic;

namespace grepapi.Models;

public partial class Registration
{
    public long Id { get; set; }

    public string? Mobile { get; set; }

    public string? Inn { get; set; }

    public string? Name { get; set; }

    public string? ShopAddress { get; set; }

    public string? BusinessAddress { get; set; }

    public string? PromotionMobile { get; set; }

    public string? PromotionInn { get; set; }

    public int? RegOpen { get; set; }
}
