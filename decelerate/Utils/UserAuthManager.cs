using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using decelerate.Utils.JWT;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using decelerate.Models;

namespace decelerate.Utils
{
    public class UserAuthManager
    {
        public UserAuthManager(IConfiguration config)
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
            var jwtUser = _jwt.Decode(sessionCookie, out errorMessage);
            /* Check payload: */
            if (jwtUser == null)
            {
                user = null;
                return false;
            }
            /* Check name and timeout against the database: */
            if (!IsUserActive(jwtUser.Name, dbContext))
            {
                user = null;
                return false;
            }
            /* Get updated user information from the database: */
            user = dbContext.Users.First(u => u.Name == jwtUser.Name);
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

        public IEnumerable<User> GetActiveUsers(DecelerateDbContext dbContext, int roomId)
        {
            return dbContext.Users.Where(u => (u.LastAction.AddSeconds(_userTimeoutSeconds) >= DateTime.UtcNow)
                && (u.RoomId == roomId)).ToList();
        }

        private readonly JWT<User> _jwt;
        private readonly int _userTimeoutSeconds;
    }
}
