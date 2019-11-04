﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using decelerate.Models;
using decelerate.Views.PresenterArea;

namespace decelerate.Controllers
{
    [Authorize]
    public class PresenterAreaController : Controller
    {
        private readonly ILogger<UserAreaController> _logger;
        private readonly AuthManager _authManager;
        private readonly DecelerateDbContext _dbContext;

        public PresenterAreaController(ILogger<UserAreaController> logger, AuthManager authManager, DecelerateDbContext dbContext)
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
            /* Delete all users: */
            _dbContext.RemoveRange(_dbContext.Users);
            _dbContext.SaveChanges();
            /* Redirect back: */
            return RedirectToAction("Index", "PresenterArea");
        }
    }
}