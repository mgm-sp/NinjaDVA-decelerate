﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using decelerate.Views.UserArea;
using decelerate.Utils.JWT;
using decelerate.Models;
using decelerate.Hubs;

namespace decelerate.Controllers
{
    public class UserAreaController : Controller
    {
        private readonly ILogger<UserAreaController> _logger;
        private readonly AuthManager _authManager;
        private readonly DecelerateDbContext _dbContext;
        private readonly IHubContext<PresenterHub> _hubContext;

        public UserAreaController(ILogger<UserAreaController> logger, AuthManager authManager, DecelerateDbContext dbContext,
            IHubContext<PresenterHub> hubContext)
        {
            _logger = logger;
            _authManager = authManager;
            _dbContext = dbContext;
            _hubContext = hubContext;
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
        public async Task<IActionResult> Index(IndexModel input)
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
                /* Notify presenter about the new vote: */
                await _hubContext.Clients.All.SendAsync("Notify", user.Name, "vote", input.SpeedChoice);
            }
            /* Show view: */
            return View(input);
        }

        public async Task<IActionResult> Logout()
        {
            /* Unregister user: */
            if (_authManager.IsAuthenticated(Request.Cookies["session"], _dbContext, out User user, out string _))
            {
                _dbContext.Users.Remove(user);
                _dbContext.SaveChanges();
            }
            /* Remove JWT cookie: */
            Response.Cookies.Append("session", "", new CookieOptions
            {
                Expires = DateTime.Now.AddDays(-1)
            });
            /* Notify presenter about the logout: */
            await _hubContext.Clients.All.SendAsync("Notify", user.Name, "logout");
            /* Redirect to home page: */
            return RedirectToAction("Index", "Home");
        }

        private bool IsAuthenticated(out IActionResult result, out User user)
        {
            if (_authManager.IsAuthenticated(Request.Cookies["session"], _dbContext, out user, out string errorMessage))
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