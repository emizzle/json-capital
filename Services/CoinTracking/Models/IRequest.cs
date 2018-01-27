using System;
using System.Collections.Generic;

namespace Services.CoinTracking.Models
{
    /// <summary>
    /// Basic necessities for any CoinTracking.info request model
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        /// API method to call
        /// </summary>
        /// <value>The method.</value>
        string Method { get; }

        /// <summary>
        /// Nonce for request that must be incremented on every API call
        /// </summary>
        /// <value>The nonce.</value>
        long Nonce { get; }

        string Key { get; }

        string Sign { get; }
    }
}
