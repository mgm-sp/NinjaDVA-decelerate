using System;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using decelerate.Views.Home;
using decelerate.Models;
using decelerate.Hubs;
using decelerate.Utils;

namespace decelerate.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserAuthManager _authManager;
        private readonly DecelerateDbContext _dbContext;
        private readonly IHubContext<PresenterHub> _hubContext;

        public HomeController(UserAuthManager authManager, DecelerateDbContext dbContext, IHubContext<PresenterHub> hubContext)
        {
            _authManager = authManager;
            _dbContext = dbContext;
            _hubContext = hubContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (_authManager.IsAuthenticated(Request.Cookies["Session"], _dbContext, out User _, out string _))
            {
                /* User is authenticated, redirect to user area: */
                return RedirectToAction("Index", "UserArea");
            }
            return View(new IndexModel
            {
                PublicRoomList = GetPublicRoomList()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IndexModel input)
        {
            input.PublicRoomList = GetPublicRoomList();
            /* Check input: */
            if (!ModelState.IsValid)
            {
                return View(input);
            }

            /* Check room: */
            Room room = null;
            if (input.RoomCode != null && input.RoomCode.Length != 0)
            {
                /* Enter via room code, check it: */
                room = _dbContext.Rooms.FirstOrDefault(r => (r.AdmissionCode == input.RoomCode));
                if (room == null)
                {
                    ViewData["RoomErrorMessage"] = "Invalid room code.";
                    return View(input);
                }
            }
            else
            {
                /* Enter a public room, check id: */
                room = _dbContext.Rooms.FirstOrDefault(r => (r.Id == input.RoomId) && (r.Public == true));
                if (room == null)
                {
                    ViewData["RoomErrorMessage"] = "Please select a room from the list or enter a room code.";
                    return View(input);
                }
            }

            /* Check if user already exists and is active: */
            if (_authManager.IsUserActive(input.Name, room.Id, _dbContext))
            {
                ModelState.AddModelError("Name", "Sorry, this name is already taken.");
                return View(input);
            }

            /* Add user to database if it doesn't exist: */
            if (_dbContext.Users.Count(u => (u.Name == input.Name) && (u.RoomId == room.Id)) == 0)
            {
                _dbContext.Add(new User { Name = input.Name, RoomId = room.Id });
                _dbContext.SaveChanges();
            }

            /* Update user info: */
            var user = _dbContext.Users.First(u => (u.Name == input.Name) && (u.RoomId == room.Id));
            user.SpeedChoice = null;
            user.LastAction = DateTime.UtcNow;
            _dbContext.Update(user);
            _dbContext.SaveChanges();

            /* Create JWT: */
            var token = _authManager.GetToken(user);

            /* Set JWT cookie: */
            Response.Cookies.Append("session", token, new CookieOptions
            {
                HttpOnly = true
            });

            /* Notify presenter about the login: */
            await _hubContext.Clients.All.SendAsync("Notify", user.Name, "login");

            /* Return response: */
            return RedirectToAction("Index", "UserArea");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private IEnumerable<Room> GetPublicRoomList()
        {
            return _dbContext.Rooms.Where(r => r.Public == true).ToList();
        }
    }
}
