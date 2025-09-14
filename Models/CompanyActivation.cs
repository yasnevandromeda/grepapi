using System;
using System.Collections.Generic;

namespace grepapi.Models;

public partial class CompanyActivation
{
    public long ActivationId { get; set; }

    public long CompanyId { get; set; }

    public long? PaymentAmount { get; set; }

    public DateTime? PaymentTimestamp { get; set; }

    public DateTime ActivationStart { get; set; }

    public DateTime ActivationEnd { get; set; }

    public int? Start { get; set; }

    public string? PaymentDescription { get; set; }

    public int? FriendsPromo { get; set; }

    public int? Payment { get; set; }
}
