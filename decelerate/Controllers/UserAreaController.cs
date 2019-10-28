using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using decelerate.Views.UserArea;
using decelerate.Utils.JWT;
using decelerate.Models;

namespace decelerate.Controllers
{
    public class UserAreaController : Controller
    {
        private readonly ILogger<UserAreaController> _logger;
        private readonly AuthManager _authManager;
        private readonly DecelerateDbContext _dbContext;

        public UserAreaController(ILogger<UserAreaController> logger, AuthManager authManager, DecelerateDbContext dbContext)
        {
            _logger = logger;
            _authManager = authManager;
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            /* Check auth: */
            if (!IsAuthenticated(out IActionResult result, out JWTPayload jwtPayload)) return result;
            /* Show view: */
            /* TODO: Get current SpeedChoice from the backend! */
            var model = new IndexModel(jwtPayload);
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(IndexModel input)
        {
            /* Check auth: */
            if (!IsAuthenticated(out IActionResult result, out JWTPayload jwtPayload)) return result;
            input.JWTpayload = jwtPayload;
            /* Check input: */
            if (ModelState.IsValid)
            {
                _logger.LogInformation($"User {jwtPayload.name} voted with {input.SpeedChoice}.");
                /* TODO: Forward new SpeedChoice to the backend! */
                ViewData["ShowModal"] = true;
            }
            /* Show view: */
            return View(input);
        }

        public IActionResult Logout()
        {
            /* Remove JWT cookie: */
            Response.Cookies.Append("session", "", new CookieOptions
            {
                Expires = DateTime.Now.AddDays(-1)
            });
            /* TODO: Unregister from backend? */
            /* Redirect to home page: */
            return RedirectToAction("Index", "Home");
        }

        private bool IsAuthenticated(out IActionResult result, out JWTPayload jwtPayload)
        {
            if (_authManager.IsAuthenticated(Request.Cookies["session"], out jwtPayload, out string errorMessage))
            {
                result = null;
                return true;
            }
            else
            {
                result = RedirectToAction("Index", "Home");
                _logger.LogWarning($"JWT Error: {errorMessage}");
                return false;
            }
        }
    }
}