using System.Collections.Generic;
using JSONCapital.Data.Models;
using JSONCapital.Services.Json.Converters;
using Newtonsoft.Json;

namespace JSONCapital.Services.CoinTracking.Models
{
    public class GetTradesResponse : Response
    {
        [JsonConverter(typeof(GetTradesResponseConverter))]
        public IEnumerable<Trade> Trades { get; set; }
    }
}
