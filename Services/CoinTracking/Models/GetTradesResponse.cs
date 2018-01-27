using System;
using System.Collections.Generic;
using JSONCapital.Data.Models;
using Newtonsoft.Json;
using Services.Json.Converters;

namespace Services.CoinTracking.Models
{
    public class GetTradesResponse : Response
    {
        [JsonConverter(typeof(TradeConverter))]
        public IEnumerable<Trade> Trades { get; set; }
    }
}
