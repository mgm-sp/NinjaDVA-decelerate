﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using decelerate.Utils.JWT;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using decelerate.Models;

namespace decelerate
{
    public class AuthManager
    {
        public AuthManager(IConfiguration config, DecelerateDbContext dbContext)
        {
            _dbContext = dbContext;
            /* Get JWT key: */
            var key = config.GetValue<string>("JwtKey");
            if (key.Length != 32)
            {
                /* Invalid key: */
                throw new ArgumentException("Invalid JwtKey configured");
            }
            /* Create JWT instance: */
            _jwt = new JWT<JWTPayload>(key);
        }

        public bool IsAuthenticated(string sessionCookie, out User user, out string errorMessage)
        {
            /* Parse JWT token: */
            var jwtPayload = _jwt.Decode(sessionCookie, out errorMessage);
            /* Check payload: */
            if (jwtPayload == null)
            {
                user = null;
                return false;
            }
            /* Check name against the database: */
            /* TODO: Check timeout! */
            if (_dbContext.Users.Count(u => u.Name == jwtPayload.name) == 0)
            {
                user = null;
                return false;
            }
            /* Return the user: */
            user = _dbContext.Users.First<User>(u => u.Name == jwtPayload.name);
            return true;
        }

        public string GetToken(JWTPayload payload)
        {
            return _jwt.Encode(payload);
        }

        private readonly JWT<JWTPayload> _jwt;
        private readonly DecelerateDbContext _dbContext;
    }
}
