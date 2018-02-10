using System.Collections.Generic;
using JSONCapital.Services.CoinTracking.Json.Converters;
using Newtonsoft.Json;

namespace JSONCapital.Services.CoinTracking.Models
{
    public class GetTradesResponse : Response
    {
        [JsonConverter(typeof(GetTradesResponseConverter))]
        public IEnumerable<CoinTrackingTrade> Trades { get; set; }
    }
}
