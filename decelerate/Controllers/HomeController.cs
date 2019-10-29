using System;
using System.Linq;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using decelerate.Views.Home;
using decelerate.Models;
using decelerate.Utils.JWT;

namespace decelerate.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AuthManager _authManager;
        private readonly DecelerateDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger, AuthManager authManager, DecelerateDbContext dbContext)
        {
            _logger = logger;
            _authManager = authManager;
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (_authManager.IsAuthenticated(Request.Cookies["Session"], out User _, out string _))
            {
                /* User is authenticated, redirect to user area: */
                return RedirectToAction("Index", "UserArea");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Index(IndexModel input)
        {
            /* Check input: */
            if (!ModelState.IsValid)
            {
                return View(input);
            }
            /* Check if user already exists and is active: */
            if (_authManager.IsUserActive(input.Name))
            {
                ViewData["ErrorMessage"] = "Sorry, this name is already taken.";
                return View(input);
            }
            /* Add user to database or update it: */
            if (_dbContext.Users.Count(u => u.Name == input.Name) == 0)
            {
                _dbContext.Add(new User { Name = input.Name, LastAction = DateTime.UtcNow });
            }
            else
            {
                var user = _dbContext.Users.First(u => u.Name == input.Name);
                user.SpeedChoice = null;
                user.LastAction = DateTime.UtcNow;
                _dbContext.Update(user);
            }
            _dbContext.SaveChanges();
            /* Create JWT: */
            var token = _authManager.GetToken(new JWTPayload(input.Name));
            /* Set JWT cookie: */
            Response.Cookies.Append("session", token, new CookieOptions
            {
                HttpOnly = true
            });
            /* Return response: */
            return RedirectToAction("Index", "UserArea");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
