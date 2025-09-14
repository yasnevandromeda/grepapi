using System;
using System.Collections.Generic;

namespace grepapi.Models;

public partial class ProductCategory
{
    public long CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;
}
