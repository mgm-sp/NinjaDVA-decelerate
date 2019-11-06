using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace decelerate.Utils
{
    public class RandomString
    {
        public static string Get(uint bytes)
        {
            byte[] data = new byte[bytes];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(data);
            }
            return BitConverter.ToString(data).Replace("-", "").ToLower();
        }
    }
}
