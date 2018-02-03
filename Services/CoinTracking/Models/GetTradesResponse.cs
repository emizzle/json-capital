using System.Collections.Generic;
using JSONCapital.Data.Json.Converters;
using JSONCapital.Data.Models;
using Newtonsoft.Json;

namespace JSONCapital.Services.CoinTracking.Models
{
    public class GetTradesResponse : Response
    {
        [JsonConverter(typeof(TradeConverter))]
        public IEnumerable<Trade> Trades { get; set; }
    }
}
