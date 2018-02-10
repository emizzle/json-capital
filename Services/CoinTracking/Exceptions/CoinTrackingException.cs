using System;
namespace JSONCapital.Services.CoinTracking.Exceptions
{
    public class CoinTrackingException : Exception
    {
        public CoinTrackingException(string message) : base(message)
        {
        }
    }
}
