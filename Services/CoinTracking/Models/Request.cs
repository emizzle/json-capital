using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JSONCapital.Common;
using JSONCapital.Common.Extensions;
using JSONCapital.Common.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using JSONCapital.Common.Helpers;
using System.Net.Http;
using System.Threading.Tasks;
using JSONCapital.Services.CoinTracking.Exceptions;
using Newtonsoft.Json;

namespace JSONCapital.Services.CoinTracking.Models
{
    public abstract class Request : IRequest
    {
        private readonly IOptions<CoinTrackingOptions> _options;
        protected string _sign = null;
        protected IEnumerable<KeyValuePair<string, object>> _signableProperties = null;
        private long? _nonce = null;
        private readonly ILogger _logger;
        private readonly JsonSerializerSettings _jsonSzrSettings;

        public Request(ILogger<Request> logger, 
                       IOptions<CoinTrackingOptions> options,
                       JsonSerializerSettings jsonSzrSettings)
        {
            _options = options;
            _logger = logger;
            _jsonSzrSettings = jsonSzrSettings;
        }

        /// <summary>
        /// Gets the nonce for the request. Must be increasing for each subsequent request.
        /// </summary>
        /// <value>The nonce.</value>
        [SignableProperty]
        public long Nonce
        {
            get
            {  
                if(_nonce == null)
                {
                    _nonce = DateTime.Now.Ticks;
                }
                return _nonce.Value;
            }
        }

        /// <summary>
        /// Gets the CoinTracking.info API method to call.
        /// </summary>
        /// <value>The method.</value>
        [SignableProperty]
        public virtual string Method => "not implemented";

        /// <summary>
        /// Gets the CoinTracking API public key
        /// </summary>
        /// <value>The key.</value>
        public string Key
        {
            get
            {
                return _options.Value.ApiPublicKey;
            }
        }

        /// <summary>
        /// Gets the result of creating a url key/value pair with all properties marked with 
        /// <see cref="SignablePropertyAttribute"/> and encrypts it using the private key with HMAC SHA512 encryption.
        /// </summary>
        /// <value>The sign.</value>
        public string Sign
        {
            get
            {
                if (_sign == null)
                {
                    var sb = new StringBuilder();

                    foreach (var signableProp in this.SignableProperties)
                    {
                        sb.AppendFormat("{0}={1}&", signableProp.Key.ToLower(), signableProp.Value);
                    }

                    var strToSign = sb.ToString().TrimEnd('&');
                    _logger.LogTrace(LoggingEvents.InformationalMarker, null, $"[Request.Sign] Generated form data string for signing: {strToSign}");
                    _sign = CryptoHelper.SignWithHmacSha512(_options.Value.ApiPrivateKey, strToSign);
                }
                return _sign;
            }
        }

        /// <summary>
        /// Gets list of properties on the Request object that are marked with <see cref="SignablePropertyAttribute"/>  
        /// </summary>
        /// <value>The signable properties.</value>
        public IEnumerable<KeyValuePair<string, object>> SignableProperties
        {
            get
            {
                if (_signableProperties == null)
                {
                    var lstProps = new List<KeyValuePair<string, object>>();
                    var props = this.GetType().GetProperties()
                                    .Where(prop => Attribute.IsDefined(prop, typeof(SignablePropertyAttribute)));

                    foreach (var prop in props)
                    {
                        var propVal = prop.GetValue(this);
                        var propType = prop.GetType();
                        if (propType.IsAssignableFrom(typeof(DateTime?)))
                        {
                            propVal = ((DateTime?)propVal).UnixTimestampFromDateTime();
                        }
                        if (propVal != null && !propVal.HasDefaultValue())
                        {
                            lstProps.Add(new KeyValuePair<string, object>(prop.Name, propVal));
                        }
                    }
                    _signableProperties = lstProps;
                }
                return _signableProperties;
            }
        }

        public async Task<T> SendRequestAsync<T>() where T : Response
        {
            using (var httpClient = new HttpClient())
            {
                var sbLogMsg = new StringBuilder("Sending CoinTracking API GetTrades request with following data:");
                sbLogMsg.AppendLine();

                var formDataContent = new MultipartFormDataContent();
                foreach (var signableProp in this.SignableProperties)
                {
                    var key = signableProp.Key.ToLower();
                    var val = signableProp.Value.ToString();
                    formDataContent.Add(new StringContent(val), key);

                    // log kvps that are being sent in request body
                    sbLogMsg.AppendLine($"{key}: {val}");
                }

                // create request
                var request = new HttpRequestMessage(HttpMethod.Post, _options.Value.ApiEndpoint) { Content = formDataContent };

                // add header vals to request
                request.Headers.Add("Key", this.Key);
                request.Headers.Add("Sign", this.Sign);

                // log header vals
                sbLogMsg.AppendLine();
                sbLogMsg.AppendLine($"Added the following header values:");
                sbLogMsg.AppendLine($"Key: {this.Key}");
                sbLogMsg.AppendLine($"Sign: {this.Sign}");

                _logger.LogTrace(sbLogMsg.ToString());

                var logMsg = "";

                var response = await httpClient.SendAsync(request);
                this._nonce = null; // clear nonce so next request it will be incremented
                this._signableProperties = null;
                this._sign = null;

                try
                {
                    _logger.LogInformation(LoggingEvents.WebRequest, null, "CoinTracking API request sent.");

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsAsync<T>(_jsonSzrSettings);


                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        logMsg = $"Error sending request ({response.StatusCode} - {response.ReasonPhrase}): {response.Content.ReadAsStringAsync().Result}";
                        _logger.LogWarning(LoggingEvents.WebRequestError, logMsg);
                        throw new CoinTrackingException(logMsg);
                    }
                }
                catch (AggregateException aggregateException)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    var aggEx = aggregateException.Flatten();
                    logMsg = $"Error sending request, {aggEx.Message}: {aggEx.StackTrace}";
                    _logger.LogWarning(LoggingEvents.WebRequestError, logMsg);
                    throw new CoinTrackingException(logMsg);
                }
            }
        }
    }
}
