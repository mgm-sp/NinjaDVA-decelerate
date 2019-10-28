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
        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpGet]
        public IActionResult Index()
        {
            /* TODO: Redirect to UserArea if user is authenticated! */
            return View();
        }

        [HttpPost]
        public IActionResult Index(IndexModel input)
        {
            /* TODO: Prevent duplicate names! */
            if (ModelState.IsValid)
            {
                /* Create JWT: */
                var key = _config.GetValue<string>("JwtKey");
                if (key.Length != 32)
                {
                    _logger.LogError("Invalid JwtKey");
                    return StatusCode(500);
                }
                var jwt = new JWT<JWTPayload>(key);
                var payload = new JWTPayload(input.Name);
                var token = jwt.Encode(payload);
                /* Set JWT cookie: */
                Response.Cookies.Append("session", token, new CookieOptions {
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
