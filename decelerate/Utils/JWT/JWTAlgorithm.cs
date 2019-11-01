using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace decelerate.Utils.JWT
{
    public enum JWTAlgorithm
    {
        None,
        HS256
    }

    public static class JWTAlgorithmMethods
    {
        public static String GetString(this JWTAlgorithm algorithm)
        {
            return algorithm switch
            {
                JWTAlgorithm.None => "none",
                JWTAlgorithm.HS256 => "HS256",
                _ => ""
            };
        }
    }
}
