using System;
using System.Collections.Generic;
using JSONCapital.Data.Models;
using Services.CoinTracking.Models;

namespace Services.CoinTracking
{
    public class Operations
    {

        /// <summary>
        /// Performs the CoinTracking.info API "getTrades" method with the parameters specified in the <paramref name="request">Request param</paramref>
        /// </summary>
        /// <returns>The trades data</returns>
        /// <param name="apiUrl">CoinTracking.info API endpoint</param>
        /// <param name="request">Request containing parmeters to send as part of the API request</param>
        public static IEnumerable<Trade> GetTrades(Uri apiUrl, GetTradesRequest request)
        {
            return null;
        }
    }
}
