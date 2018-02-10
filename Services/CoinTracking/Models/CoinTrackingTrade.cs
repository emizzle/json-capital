using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JSONCapital.Common.Extensions;
using Newtonsoft.Json;

namespace JSONCapital.Services.CoinTracking.Models
{
    public class CoinTrackingTrade
    {
        public enum TradeTypeEnum
        {
            Trade,
            Deposit,
            Withdrawal,
            Income,
            Mining,
            Gift_Or_Tip__In,
            Spend,
            Donation,
            Gift__Out,
            Stolen,
            Lost
        }

        /// <summary>
        /// Gets or sets the the unique CoinTracking transaction id.
        /// </summary>
        /// <value>The unique CoinTracking transaction id.</value>
        [Key]
        public int CoinTrackingTradeID { get; set; }

        /// <summary>
        /// Gets or sets the the bought or received amount.
        /// </summary>
        /// <value>The the bought or received amount.</value>
        [JsonProperty("buy_amount")]
        public float? BuyAmount { get; set; }

        /// <summary>
        /// Gets or sets the bought or received currency.
        /// </summary>
        /// <value>The bought or received currency.</value>
        [JsonProperty("buy_currency")]
        public string BuyCurrency { get; set; }

        /// <summary>
        /// Gets or sets the sold or withdrawn amount.
        /// </summary>
        /// <value>The sold or withdrawn amount.</value>
        [JsonProperty("sell_amount")]
        public float? SellAmount { get; set; }

        /// <summary>
        /// Gets or sets the sold or withdrawn currency.
        /// </summary>
        /// <value>The sold or withdrawn currency.</value>
        [JsonProperty("sell_currency")]
        public string SellCurrency { get; set; }

        /// <summary>
        /// Gets or sets the fee amount.
        /// </summary>
        /// <value>The fee amount.</value>
        [JsonProperty("fee_amount")]
        public float? FeeAmount { get; set; }

        /// <summary>
        /// Gets or sets the fee currency.
        /// </summary>
        /// <value>The fee currency.</value>
        [JsonProperty("fee_currency")]
        public string FeeCurrency { get; set; }

        /// <summary>
        /// Gets or sets the type of the trade.
        /// </summary>
        /// <value>The type of the trade.</value>
        [NotMapped]
        public TradeTypeEnum TradeType { get; set; }

        /// <summary>
        /// Gets or sets the trade type via it's string value.
        /// </summary>
        /// <value>The trade type string.</value>
        [Column("TradeType")]
        [JsonProperty("type")]
        public string TradeTypeString
        {
            get { return TradeType.ToString(); }
            set { TradeType = value.ParseEnum<TradeTypeEnum>(); }
        }

        /// <summary>
        /// Gets or sets the exchange set on CoinTracking.
        /// </summary>
        /// <value>The exchange set on CoinTracking.</value>
        [JsonProperty("exchange")]
        public string Exchange { get; set; }

        /// <summary>
        /// Gets or sets the trade group set on CoinTracking.
        /// </summary>
        /// <value>The trade group set on CoinTracking.</value>
        [JsonProperty("group")]
        public string Group { get; set; }

        /// <summary>
        /// Gets or sets the comment set on CoinTracking.
        /// </summary>
        /// <value>The comment set on CoinTracking.</value>
        [JsonProperty("comment")]
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the name of the exchange, this transaction was imported from. (e.g. kraken). API imports start with 'job' (e.g. job_kraken). Only for imported transactions..
        /// </summary>
        /// <value> The name of the exchange, this transaction was imported from. (e.g. kraken). API imports start with 'job' (e.g. job_kraken). Only for imported transactions..</value>
        [JsonProperty("imported_from")]
        public string ImportedFrom { get; set; }

        /// <summary>
        /// Gets or sets the UNIX timestamp of the transaction (UNIX timestamp are always in UTC).
        /// </summary>
        /// <value>The UNIX timestamp of the transaction (UNIX timestamp are always in UTC).</value>
        [JsonProperty("time")]
        //[JsonConverter(typeof(DateTimeConverter))]
        public DateTime? Time { get; set; }

        /// <summary>
        /// Gets or sets the UNIX timestamp this transaction was added to CoinTracking.
        /// </summary>
        /// <value>The UNIX timestamp this transaction was added to CoinTracking.</value>
        //[JsonConverter(typeof(DateTimeConverter))]
        [JsonProperty("imported_time")]
        public DateTime? ImportedTime { get; set; }

        /// <summary>
        /// Gets or sets the trade id of the exchange. Only for imported transactions.
        /// </summary>
        /// <value>The trade id of the exchange.</value>
        [JsonProperty("trade_id")]
        public string ImportedTradeID { get; set; }
    }
}
