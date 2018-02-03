using System;
using System.ComponentModel.DataAnnotations;
using JSONCapital.Common.Json.Converters;
using Newtonsoft.Json;

namespace JSONCapital.Data.Models
{
    public class Trade
    {
        public Trade()
        {
        }
        public enum TradeTypeEnum
        {
            Trade,
            Deposit
        }
        [Key]
        public int TradeID { get; set; }

        [JsonProperty("buy_amount")]
        public float BuyAmount { get; set; }

        [JsonProperty("buy_currency")]
        public string BuyCurrency { get; set; }

        [JsonProperty("sell_amount")]
        public float SellAmount { get; set; }

        [JsonProperty("sell_currency")]
        public string SellCurrency { get; set; }

        [JsonProperty("fee_amount")]
        public float FeeAmount { get; set; }

        [JsonProperty("fee_currency")]
        public string FeeCurrency { get; set; }

        [JsonProperty("type")]
        public TradeTypeEnum TradeType { get; set; }

        [JsonProperty("exchange")]
        public string Exchange { get; set; }

        [JsonProperty("group")]
        public string Group { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("imported_from")]
        public string ImportedFrom { get; set; }

        [JsonConverter(typeof(DateTimeConverter))]
        [JsonProperty("imported_time")]
        public DateTime ImportedTime { get; set; }

		[JsonProperty("trade_id")]
		public string CoinTrackingTradeID { get; set; }
    }
}
