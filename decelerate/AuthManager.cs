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
            var key = config.GetValue<string>("JwtKey");
            if (key.Length != 32)
            {
                /* Invalid key: */
                throw new ArgumentException("Invalid JwtKey configured");
            }
            /* Create JWT instance: */
            jwt = new JWT<JWTPayload>(key);
        }

        public bool IsAuthenticated(string sessionCookie, out JWTPayload jwtPayload, out string errorMessage)
        {
            /* Parse JWT token: */
            jwtPayload = jwt.Decode(sessionCookie, out errorMessage);
            /* Check payload: */
            return (jwtPayload != null);
        }

        public string GetToken(JWTPayload payload)
        {
            return jwt.Encode(payload);
        }

        private readonly JWT<JWTPayload> jwt;
    }
}
