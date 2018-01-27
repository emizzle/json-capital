using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JSONCapital.Common.Options;
using Microsoft.Extensions.Options;
using Services.Helpers;

namespace Services.CoinTracking.Models
{
    public abstract class Request : IRequest
    {
        private readonly IOptions<CoinTrackingOptions> _options;
        private string mSign = null;
        private IEnumerable<KeyValuePair<string, object>> mSignableProperties = null;

        public Request(IOptions<CoinTrackingOptions> options)
        {
            _options = options;
        }

        [SignableProperty]
        public long Nonce
        {
            get
            {
                return DateTime.Now.Ticks;
            }
        }

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
                if (mSign == null)
                {
                    var sb = new StringBuilder();

                    foreach (var signableProp in this.SignableProperties)
                    {
                        sb.AppendFormat("{0}={1}&", signableProp.Key.ToLower(), signableProp.Value.ToString().ToLower());
                    }

                    mSign = CryptoHelper.SignWithHmacSha512(_options.Value.ApiPrivateKey, sb.ToString());
                }
                return mSign;
            }
        }

        public IEnumerable<KeyValuePair<string, object>> SignableProperties
        {
            get
            {
                if (mSignableProperties == null)
                {
                    mSignableProperties = this.GetType().GetProperties()
                                              .Where(prop => Attribute.IsDefined(prop, typeof(SignablePropertyAttribute)))
                                              .Select(prop => new KeyValuePair<string, object>(prop.Name, prop.GetValue(this)));
                }
                return mSignableProperties;
            }
        }
    }
}
