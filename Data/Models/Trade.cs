using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using JSONCapital.Common.Extensions;
using JSONCapital.Common.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace JSONCapital.Data.Models
{
    public class Trade
    {
        public Trade()
        {
        }

        //TODO: test that this can be parsed and serialized as a string
        public enum TradeTypeEnum
        {
            Trade,
            Deposit,
            Withdrawal,
            Income,
            Mining,
            [EnumMember(Value = "Gift/Tip(In)")]
            Gift_Or_Tip__In,
            Spend,
            Donation,
            [EnumMember(Value = "Gift(Out)")]
            Gift__Out,
            Stolen,
            Lost
        }

        /// <summary>
        /// Gets or sets the unique DB trade identifier.
        /// </summary>
        /// <value>The unique DB trade identifier.</value>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TradeID { get; set; }

        /// <summary>
        /// Gets or sets the the unique CoinTracking transaction id.
        /// </summary>
        /// <value>The unique CoinTracking transaction id.</value>
        public int CoinTrackingTradeID { get; set; }

        /// <summary>
        /// Gets or sets the the bought or received amount.
        /// </summary>
        /// <value>The the bought or received amount.</value>
        public float? BuyAmount { get; set; }

        /// <summary>
        /// Gets or sets the bought or received currency.
        /// </summary>
        /// <value>The bought or received currency.</value>
        public string BuyCurrency { get; set; }

        /// <summary>
        /// Gets or sets the sold or withdrawn amount.
        /// </summary>
        /// <value>The sold or withdrawn amount.</value>
        public float? SellAmount { get; set; }

        /// <summary>
        /// Gets or sets the sold or withdrawn currency.
        /// </summary>
        /// <value>The sold or withdrawn currency.</value>
        public string SellCurrency { get; set; }

        /// <summary>
        /// Gets or sets the fee amount.
        /// </summary>
        /// <value>The fee amount.</value>
        public float? FeeAmount { get; set; }

        /// <summary>
        /// Gets or sets the fee currency.
        /// </summary>
        /// <value>The fee currency.</value>
        public string FeeCurrency { get; set; }

        /// <summary>
        /// Gets or sets the type of the trade.
        /// </summary>
        /// <value>The type of the trade.</value>
        [NotMapped]
        [JsonIgnore]
        public TradeTypeEnum TradeType { get; set; }

        /// <summary>
        /// Gets or sets the trade type via it's string value.
        /// </summary>
        /// <value>The trade type string.</value>
        [Column("TradeType")]
        [JsonProperty("tradeType")]
        public string TradeTypeString
        {
            get { return TradeType.GetAttribute<EnumMemberAttribute>()?.Value ?? TradeType.ToString(); }
            set { TradeType = value.ParseEnum<TradeTypeEnum>(); }
        }

        /// <summary>
        /// Gets or sets the exchange set on CoinTracking.
        /// </summary>
        /// <value>The exchange set on CoinTracking.</value>
        public string Exchange { get; set; }

        /// <summary>
        /// Gets or sets the trade group set on CoinTracking.
        /// </summary>
        /// <value>The trade group set on CoinTracking.</value>
        public string Group { get; set; }

        /// <summary>
        /// Gets or sets the comment set on CoinTracking.
        /// </summary>
        /// <value>The comment set on CoinTracking.</value>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the name of the exchange, this transaction was imported from. (e.g. kraken). API imports start with 'job' (e.g. job_kraken). Only for imported transactions..
        /// </summary>
        /// <value> The name of the exchange, this transaction was imported from. (e.g. kraken). API imports start with 'job' (e.g. job_kraken). Only for imported transactions..</value>
        public string ImportedFrom { get; set; }

        /// <summary>
        /// Gets or sets the UNIX timestamp of the transaction (UNIX timestamp are always in UTC).
        /// </summary>
        /// <value>The UNIX timestamp of the transaction (UNIX timestamp are always in UTC).</value>
        //[JsonConverter(typeof(DateTimeConverter))]
        public DateTime? Time { get; set; }

        /// <summary>
        /// Gets or sets the UNIX timestamp this transaction was added to CoinTracking.
        /// </summary>
        /// <value>The UNIX timestamp this transaction was added to CoinTracking.</value>
        public DateTime? ImportedTime { get; set; }

        /// <summary>
        /// Gets or sets the trade id of the exchange. Only for imported transactions.
        /// </summary>
        /// <value>The trade id of the exchange.</value>
		public string ImportedTradeID { get; set; }
    }
}
