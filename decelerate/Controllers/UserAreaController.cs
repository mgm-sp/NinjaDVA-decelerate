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

        private bool IsAuthenticated(out IActionResult result, out JWTPayload jwtPayload)
        {
            /* Parse JWT token: */
            var key = _config.GetValue<string>("JwtKey");
            if (key.Length != 32)
            {
                /* Invalid key, return false and 500 error: */
                _logger.LogError("Invalid JwtKey");
                result = StatusCode(500);
                jwtPayload = null;
                return false;
            }
            var jwt = new JWT<JWTPayload>(key);
            jwtPayload = jwt.Decode(Request.Cookies["session"], out string errorMessage);
            /* Check payload: */
            if (jwtPayload == null)
            {
                /* Not authenticated, return false and redirect: */
                _logger.LogWarning($"JWT Error: {errorMessage}");
                result = RedirectToAction("Index", "Home");
                return false;
            }
            /* Authenticated, return true and no result: */
            result = null;
            return true;
        }
    }
}