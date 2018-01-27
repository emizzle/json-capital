using System;
using System.Text;
using System.Security.Cryptography;

namespace Services.Helpers
{
    public class CryptoHelper
    {
        public static string SignWithHmacSha512(string privateKey, string message)
        {
            var keyBytes = Encoding.UTF8.GetBytes(privateKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            // Initialize the keyed hash object.
            using (HMACSHA512 hmac = new HMACSHA512(keyBytes))
            {
                // Compute the hash of the message
                byte[] hashValue = hmac.ComputeHash(messageBytes);

                // get the string value of the signed message
                return Encoding.UTF8.GetString(hashValue);
            }
        }
    }
}
