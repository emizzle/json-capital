using System;
namespace JSONCapital.Common.Options
{
    public class JSONCapitalOptions
    {
        public ConnectionStringOptions ConnectionStrings { get; set; }
        public CoinTrackingOptions CoinTracking { get; set; }
    }
}
