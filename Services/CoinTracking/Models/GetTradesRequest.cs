using System;
using JSONCapital.Common.Options;
using Microsoft.Extensions.Options;

namespace Services.CoinTracking.Models
{
    /// <summary>
    /// Model for populating a "getTrades" request for the CoinTracking.info API.
    /// </summary>
    public class GetTradesRequest : Request
    {
        public GetTradesRequest(IOptions<CoinTrackingOptions> options) : base(options)
        {

        }

        /// <summary>
        /// Order direction options
        /// </summary>
        public enum OrderDirection
        {
            ASC,
            DESC
        }

        /// <summary>
        /// CoinTracking.info API method to call
        /// </summary>
        /// <value>The method.</value>
        [SignableProperty]
        public override string Method => "getTrades";

        /// <summary>
        /// Number of trades
        /// </summary>
        /// <value>The limit.</value>
        [SignableProperty]
        public int Limit { get; set; }

        /// <summary>
        /// ASC or DESC order by trade time
        /// </summary>
        /// <value>The order.</value>
        [SignableProperty]
        public OrderDirection Order { get; set; }

        /// <summary>
        /// timestamp as trade start date
        /// </summary>
        /// <value>The trade start date.</value>
        [SignableProperty]
        public DateTime TradeStartDate { get; set; }

        /// <summary>
        /// timestamp as trade end date
        /// </summary>
        /// <value>The trade end date.</value>
        [SignableProperty]
        public DateTime TradeEndDate { get; set; }
    }
}
