using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using decelerate.Models;

namespace decelerate.Utils
{
    public class PresenterAuthHelper
    {
        public static Presenter GetPresenter(DecelerateDbContext dbContext, ClaimsPrincipal user)
        {
            /* Get presenter from database and fetch the rooms: */
            var presenter = dbContext.Presenters.First(p => p.Name == user.Identity.Name);
            dbContext.Entry(presenter).Collection(p => p.Rooms).Load();
            return presenter;
        }

        public static Room GetRoom(int roomId, DecelerateDbContext dbContext, ClaimsPrincipal user)
        {
            /* Check if room exists: */
            var room = dbContext.Rooms.FirstOrDefault(r => r.Id == roomId);
            if (room == null)
            {
                return null;
            }

            /* Check if presenter owns the room: */
            if (room.PresenterName != user.Identity.Name)
            {
                return null;
            }

            /* Fetch the users: */
            dbContext.Entry(room).Collection(r => r.Users).Load();

            return room;
        }
    }
}
