using System;
namespace JSONCapital.Services.Exceptions
{
    public class CoinTrackingException : Exception
    {
        public CoinTrackingException(string message) : base(message)
        {
        }
    }
}
