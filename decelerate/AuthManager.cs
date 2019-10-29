using System;
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
            /* Get user timeout: */
            _userTimeoutSeconds = config.GetValue<int>("UserTimeoutSeconds");
            if (_userTimeoutSeconds <= 0)
            {
                /* Invalid timeout: */
                throw new ArgumentException("Invalid UserTimeoutSeconds configured");
            }
        }

        public bool IsUserActive(string name)
        {
            var count = _dbContext.Users.Count(u => (u.Name == name) &&
                (u.LastAction.AddSeconds(_userTimeoutSeconds) >= DateTime.UtcNow));
            return (count != 0);
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
            /* Check name and timeout against the database: */
            if (!IsUserActive(jwtPayload.name))
            {
                user = null;
                return false;
            }
            /* Get the user: */
            user = _dbContext.Users.First(u => u.Name == jwtPayload.name);
            /* Update last action: */
            user.LastAction = DateTime.UtcNow;
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();
            /* Return success: */
            return true;
        }

        public string GetToken(JWTPayload payload)
        {
            return _jwt.Encode(payload);
        }

        private readonly JWT<JWTPayload> _jwt;
        private readonly DecelerateDbContext _dbContext;
        private readonly int _userTimeoutSeconds;
    }
}
