using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class Trade
    {
        public Trade()
        {
        }
        public enum TradeTypeEnum
        {
            Buy,
            Sell
        }
        [Key]
        public int TradeID { get; set; }
        public TradeTypeEnum TradeType { get; set; }
        public string TradeCurrency { get; set; }
        public string TradeCurrencyBasePair { get; set; }
    }
}
