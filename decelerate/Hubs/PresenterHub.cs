using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using decelerate.Utils;
using decelerate.Models;

namespace decelerate.Hubs
{
    [Authorize]
    public class PresenterHub : Hub
    {
        private readonly DecelerateDbContext _dbContext;

        public PresenterHub(DecelerateDbContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task JoinRoom(int id)
        {
            /* Check access rights and get room: */
            var room = PresenterAuthHelper.GetRoom(id, _dbContext, Context.User);
            if (room == null)
            {
                Context.Abort();
            }

            /* Join room: */
            await Groups.AddToGroupAsync(Context.ConnectionId, $"rooms/{room.Id}");
        }
    }
}
