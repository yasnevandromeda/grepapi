using System;
using System.Collections.Generic;

namespace grepapi.Models;

public partial class CompanyPromoRequest
{
    public long Id { get; set; }

    public long CompanyId { get; set; }

    public string? Inn { get; set; }

    public string? Phone { get; set; }

    public int? Active { get; set; }

    public DateTime? Ts { get; set; }
}
