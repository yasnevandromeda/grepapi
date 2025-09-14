using System;
using System.Collections.Generic;

namespace grepapi.Models;

public partial class Company
{
    public long CompanyId { get; set; }

    public string Inn { get; set; } = null!;

    public string? BusinessMobile { get; set; }

    public string? BusinessEmail { get; set; }

    public string CompanyPassword { get; set; } = null!;

    public DateTime RegistrationTs { get; set; }

    public string? FriendInn { get; set; }

    public string? FriendPhone { get; set; }

    public string? BusinessAddress { get; set; }

    public string? CompanyName { get; set; }

    public int CompanyType { get; set; }
}
