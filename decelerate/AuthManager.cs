using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using decelerate.Utils.JWT;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace decelerate
{
    public class AuthManager
    {
        public AuthManager(IConfiguration config)
        {
            /* Get JWT key: */
            key = config.GetValue<string>("JwtKey");
            if (key.Length != 32)
            {
                /* Invalid key: */
                throw new ArgumentException("Invalid JwtKey configured");
            }
        }

        public bool IsAuthenticated(string sessionCookie, out JWTPayload jwtPayload, out string errorMessage)
        {
            /* Parse JWT token: */
            var jwt = new JWT<JWTPayload>(key);
            jwtPayload = jwt.Decode(sessionCookie, out errorMessage);
            /* Check payload: */
            if (jwtPayload == null)
            {
                /* Not authenticated: */
                return false;
            }
            /* Authenticated: */
            return true;
        }

        private readonly string key;
    }
}
