using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using decelerate.Models;
using decelerate.Views.PresenterArea;

namespace decelerate.Controllers
{
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
            /* TODO: Add authentication! */
            return View(GetData());
        }

        public IActionResult Poll()
        {
            /* TODO: Add authentication! */
            return new ObjectResult(GetData());
        }

        private IndexModel GetData()
        {
            /* Get users: */
            var users = _authManager.GetActiveUsers();
            /* Calculate average speed choice: */
            int sumSpeedChoice = 0;
            uint cntSpeedChoice = 0;
            foreach (var user in users)
            {
                if (user.SpeedChoice != null)
                {
                    sumSpeedChoice += user.SpeedChoice ?? 0;
                    cntSpeedChoice++;
                }
            }
            var avgSpeedChoice = (cntSpeedChoice != 0) ? ((float)sumSpeedChoice / cntSpeedChoice) : 0;
            /* Return model: */
            return new IndexModel
            {
                Users = users,
                AverageSpeedChoice = (int)avgSpeedChoice
            };
        }
    }
}