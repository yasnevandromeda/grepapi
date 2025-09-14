using System;
using System.Collections.Generic;

namespace grepapi.Models;

public partial class Catalog
{
    public long CatalogId { get; set; }

    public string? CatalogName { get; set; }

    public long? CompanyId { get; set; }
}
