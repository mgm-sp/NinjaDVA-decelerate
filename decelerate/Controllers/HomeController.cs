using System;
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
            if (_authManager.IsAuthenticated(Request.Cookies["Session"], out JWTPayload _, out string _))
            {
                /* User is authenticated, redirect to user area: */
                return RedirectToAction("Index", "UserArea");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Index(IndexModel input)
        {
            /* TODO: Prevent duplicate names! */
            if (ModelState.IsValid)
            {
                /* Add user to database: */
                _dbContext.Add(new User { Name = input.Name });
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
            return View(input);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
