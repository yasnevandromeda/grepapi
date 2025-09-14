
using grepapi.Models;

namespace grepapi.SearchModel
{
    public class MarketBrief
    {
        public MarketBrief() { }

        public MarketBrief(Market m)
        {
            MarketId = m.MarketId;
            MarketName = m.MarketName ?? "";
            LocationLatt = m.LocationGeo.HasValue ? m.LocationGeo.Value.X : 0;
            LocationLong = m.LocationGeo.HasValue ? m.LocationGeo.Value.Y : 0;
            GeoLink = m.GeoLink ?? "";
            MarketWeb = m.MarketWeb ?? "";
            ProfileType = m.ProfileType;
        }

        public long MarketId { get; set; }

        public string MarketName { get; set; } = "";

        public double LocationLatt { get; set; }

        public double LocationLong { get; set; }

        public string GeoLink { get; set; } = "";

        public string MarketWeb { get; set; } = "";

        public long ProfileType { get; set; } = 2;
    }
}
