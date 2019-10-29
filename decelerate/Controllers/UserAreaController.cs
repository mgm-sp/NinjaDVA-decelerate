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
            if (!IsAuthenticated(out IActionResult result, out User user)) return result;
            /* Show view: */
            var model = new IndexModel { User = user };
            model.SpeedChoice = user.SpeedChoice ?? 0;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(IndexModel input)
        {
            /* Check auth: */
            if (!IsAuthenticated(out IActionResult result, out User user)) return result;
            input.User = user;
            /* Check input: */
            if (ModelState.IsValid)
            {
                /* Save new speed choice: */
                user.SpeedChoice = input.SpeedChoice;
                _dbContext.Users.Update(user);
                _dbContext.SaveChanges();
                /* Show success message to the user: */
                ViewData["ShowModal"] = true;
            }
            /* Show view: */
            return View(input);
        }

        public IActionResult Logout()
        {
            /* Unregister user: */
            if (_authManager.IsAuthenticated(Request.Cookies["session"], out User user, out string _))
            {
                _dbContext.Users.Remove(user);
                _dbContext.SaveChanges();
            }
            /* Remove JWT cookie: */
            Response.Cookies.Append("session", "", new CookieOptions
            {
                Expires = DateTime.Now.AddDays(-1)
            });
            /* Redirect to home page: */
            return RedirectToAction("Index", "Home");
        }

        private bool IsAuthenticated(out IActionResult result, out User user)
        {
            if (_authManager.IsAuthenticated(Request.Cookies["session"], out user, out string errorMessage))
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