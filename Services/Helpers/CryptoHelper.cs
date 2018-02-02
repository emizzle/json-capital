using System;
using System.Text;
using System.Security.Cryptography;
//using Org.BouncyCastle.Crypto.Macs;
//using Org.BouncyCastle.Crypto.Digests;
//using Org.BouncyCastle.Crypto.Parameters;

namespace JSONCapital.Services.Helpers
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
                return BitConverter.ToString(hashValue).Replace("-", "").ToLower();
            }

            //var hmac = new HMac(new Sha512Digest());
            //hmac.Init(new KeyParameter(Encoding.UTF8.GetBytes(privateKey)));
            //byte[] result = new byte[hmac.GetMacSize()];
            //byte[] bytes = Encoding.UTF8.GetBytes(message);

            //hmac.BlockUpdate(bytes, 0, bytes.Length);
            //hmac.DoFinal(result, 0);

            //return BitConverter.ToString(result);
        }
    }
}
