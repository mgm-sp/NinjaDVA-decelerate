using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using decelerate.Views.UserArea;
using decelerate.Utils.JWT;

namespace decelerate.Controllers
{
    public class UserAreaController : Controller
    {
        private readonly ILogger<UserAreaController> _logger;
        private readonly IConfiguration _config;

        public UserAreaController(ILogger<UserAreaController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }
        public IActionResult Index()
        {
            /* Parse JWT token: */
            var key = _config.GetValue<string>("JwtKey");
            if (key.Length != 32)
            {
                _logger.LogError("Invalid JwtKey");
                return StatusCode(500);
            }
            var jwt = new JWT<JWTPayload>(key);
            var jwtPayload = jwt.Decode(Request.Cookies["session"], out string errorMessage);
            if (jwtPayload == null)
            {
                _logger.LogWarning($"JWT Error: {errorMessage}");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var model = new IndexModel(jwtPayload);
                return View(model);
            }
        }
    }
}