using System;
using Newtonsoft.Json;
using Services.Json.Converters;

namespace Services.CoinTracking.Models
{
    public class Response
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Services.CoinTracking.Response"/> is success.
        /// </summary>
        /// <value><c>true</c> if success; otherwise, <c>false</c>.</value>
        [JsonConverter(typeof(BooleanConverter))]
        public bool Success { get; set; }

        /// <summary>
        /// CoinTracking.info API method called in the request
        /// </summary>
        /// <value>The method.</value>
        public string Method { get; set; }

        /// <summary>
        /// CoinTracking.info API response error code. This will be null if there is no error.
        /// </summary>
        /// <value>The error.</value>
        public string Error { get; set; }

        /// <summary>
        /// CoinTracking.info API response error message. This will be null if there is no error.
        /// </summary>
        /// <value>The error message.</value>
        [JsonProperty("error_msg")]
        public string ErrorMessage { get; set; }
    }
}
