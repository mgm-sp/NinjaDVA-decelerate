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
            _jwt = new JWT<User>(key, JWTAlgorithm.HS256);
            /* Get user timeout: */
            _userTimeoutSeconds = config.GetValue<int>("UserTimeoutSeconds");
            if (_userTimeoutSeconds <= 0)
            {
                /* Invalid timeout: */
                throw new ArgumentException("Invalid UserTimeoutSeconds configured");
            }
        }

        public bool IsUserActive(string name, DecelerateDbContext dbContext)
        {
            var count = dbContext.Users.Count(u => (u.Name == name) &&
                (u.LastAction.AddSeconds(_userTimeoutSeconds) >= DateTime.UtcNow));
            return (count != 0);
        }

        public bool IsAuthenticated(string sessionCookie, DecelerateDbContext dbContext, out User user, out string errorMessage)
        {
            /* Parse JWT token: */
            user = _jwt.Decode(sessionCookie, out errorMessage);
            /* Check payload: */
            if (user == null)
            {
                return false;
            }
            /* Check name and timeout against the database: */
            if (!IsUserActive(user.Name, dbContext))
            {
                user = null;
                return false;
            }
            /* Update last action: */
            user.LastAction = DateTime.UtcNow;
            dbContext.Users.Update(user);
            dbContext.SaveChanges();
            /* Return success: */
            return true;
        }

        public string GetToken(User user)
        {
            return _jwt.Encode(user);
        }

        public IEnumerable<User> GetActiveUsers(DecelerateDbContext dbContext)
        {
            return dbContext.Users.Where(u => u.LastAction.AddSeconds(_userTimeoutSeconds) >= DateTime.UtcNow).ToList();
        }

        private readonly JWT<User> _jwt;
        private readonly int _userTimeoutSeconds;
    }
}
