using System;
using System.Collections.Generic;

namespace grepapi.Models;

public partial class TradeSegment
{
    public long SegmentId { get; set; }

    public string SegmentName { get; set; } = null!;
}
