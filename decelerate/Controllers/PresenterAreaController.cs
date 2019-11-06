﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using decelerate.Models;
using decelerate.Views.PresenterArea;
using decelerate.Utils;

namespace decelerate.Controllers
{
    [Authorize]
    public class PresenterAreaController : Controller
    {
        private readonly ILogger<UserAreaController> _logger;
        private readonly UserAuthManager _authManager;
        private readonly DecelerateDbContext _dbContext;

        public PresenterAreaController(ILogger<UserAreaController> logger, UserAuthManager authManager, DecelerateDbContext dbContext)
        {
            _logger = logger;
            _authManager = authManager;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View(new IndexModel { Users = _authManager.GetActiveUsers(_dbContext) });
        }

        public IActionResult Poll()
        {
            return new ObjectResult(new IndexModel { Users = _authManager.GetActiveUsers(_dbContext) });
        }

        public IActionResult ClearVotes()
        {
            /* TODO: Fix CSRF vulnerability? */
            /* Set all votes to NULL: */
            foreach (var user in _dbContext.Users)
            {
                user.SpeedChoice = null;
            }
            _dbContext.SaveChanges();
            /* Redirect back: */
            return RedirectToAction("Index", "PresenterArea");
        }

        public IActionResult ClearUsers()
        {
            /* TODO: Fix CSRF vulnerability? */
            /* Delete all users: */
            _dbContext.RemoveRange(_dbContext.Users);
            _dbContext.SaveChanges();
            /* Redirect back: */
            return RedirectToAction("Index", "PresenterArea");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            /* TODO: Redirect if already logged in. */
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginModel input)
        {
            /* Check input: */
            if (!ModelState.IsValid)
            {
                return View(input);
            }

            /* Check if presenter exists: */
            var invalid = false;
            var presenter = _dbContext.Presenters.FirstOrDefault(p => p.Name == input.Username.ToLower());
            if (presenter == null)
            {
                invalid = true;
            }

            /* Check presenter password (even if the username is invalid): */
            var passwordHash = (presenter == null) ? "$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy" : presenter.PasswordHash;
            if (!BCrypt.Net.BCrypt.Verify(input.Password, passwordHash))
            {
                invalid = true;
            }

            /* Return error if username or password is invalid: */
            if (invalid)
            {
                ViewData["ErrorMessage"] = "Invalid username or password.";
                return View(input);
            }

            /* TODO: Create session and redirect! */
            return null;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            /* TODO: Redirect if already logged in. */
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterModel input)
        {
            /* Check input: */
            if (!ModelState.IsValid)
            {
                return View(input);
            }

            /* Check if presenter already exists: */
            if (_dbContext.Presenters.Count(p => p.Name == input.Username.ToLower()) != 0)
            {
                ModelState.AddModelError("Username", "This username is already taken.");
                return View(input);
            }

            /* Register presenter: */
            _dbContext.Presenters.Add(new Presenter
            {
                Name = input.Username.ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(input.Password)
            });

            /* TODO: Create session and redirect! */
            return null;
        }
    }
}