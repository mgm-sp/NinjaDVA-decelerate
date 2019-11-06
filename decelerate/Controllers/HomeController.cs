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
using decelerate.Utils.JWT;

namespace decelerate.Controllers
{
    public class HomeController : Controller
    {
        private readonly AuthManager _authManager;
        private readonly DecelerateDbContext _dbContext;
        private readonly IHubContext<PresenterHub> _hubContext;

        public HomeController(AuthManager authManager, DecelerateDbContext dbContext, IHubContext<PresenterHub> hubContext)
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
            /* Check if user already exists and is active: */
            if (_authManager.IsUserActive(input.Name, _dbContext))
            {
                ViewData["ErrorMessage"] = "Sorry, this name is already taken.";
                return View(input);
            }
            /* Add user to database if it doesn't exist: */
            if (_dbContext.Users.Count(u => u.Name == input.Name) == 0)
            {
                _dbContext.Add(new User { Name = input.Name });
                _dbContext.SaveChanges();
            }
            /* Update user info: */
            var user = _dbContext.Users.First(u => u.Name == input.Name);
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
            /* TODO: Implement! */
            return new List<Room>
            {
                new Room { Id = 1, Name = "Hardcoded test room" }
            };
        }
    }
}
