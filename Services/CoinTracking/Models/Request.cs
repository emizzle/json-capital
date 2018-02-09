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
using JSONCapital.Services.Helpers;

namespace JSONCapital.Services.CoinTracking.Models
{
    public abstract class Request : IRequest
    {
        private readonly IOptions<CoinTrackingOptions> _options;
        protected string _sign = null;
        protected IEnumerable<KeyValuePair<string, object>> _signableProperties = null;
        private long? _nonce = null;
        private readonly ILogger _logger;

        public Request(ILogger<Request> logger, IOptions<CoinTrackingOptions> options)
        {
            _options = options;
            _logger = logger;
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

        public string Key
        {
            get
            {
                return _options.Value.ApiPublicKey;
            }
        }

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

        /// <summary>
        /// Resets the nonce backing variable so the next time the nonce is
        /// retrieved, it is updated.
        /// </summary>
        public void ClearNonce()
        {
            this._nonce = null;
        }
    }
}
