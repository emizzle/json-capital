using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JSONCapital.Common.Extensions;
using JSONCapital.Common.Options;
using Microsoft.Extensions.Options;
using Services.Helpers;

namespace Services.CoinTracking.Models
{
    public abstract class Request : IRequest
    {
        private readonly IOptions<CoinTrackingOptions> _options;
        protected string mSign = null;
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

                    mSign = CryptoHelper.SignWithHmacSha512(_options.Value.ApiPrivateKey, sb.ToString().TrimEnd('&'));
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
                        if (propVal != null && !propVal.HasDefaultValue())//, default(prop.GetType()))// propVal.Equals(default(typeof(propType)))
                        {
                            lstProps.Add(new KeyValuePair<string, object>(prop.Name, propVal));
                        }
                    }
                    mSignableProperties = lstProps;
                }
                return mSignableProperties;
            }
        }
    }
}
