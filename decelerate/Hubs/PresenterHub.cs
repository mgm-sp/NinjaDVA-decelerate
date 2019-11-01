using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace decelerate.Hubs
{
    [Authorize]
    public class PresenterHub : Hub
    {
    }
}
