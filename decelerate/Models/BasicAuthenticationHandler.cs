using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Text.Encodings.Web;
using System.Security.Claims;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

/**
 * based on
 * https://jasonwatmore.com/post/2019/10/21/aspnet-core-3-basic-authentication-tutorial-with-example-api
 */
namespace decelerate.Models
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IConfiguration config
        ) : base(options, logger, encoder, clock)
        {
            /* Get username & password from config: */
            _correctUsername = "presenter";
            _correctPassword = "supersecret";
            /* TODO: Use credentials from the database! */
        }

#pragma warning disable CS1998 // missing await operators
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
#pragma warning restore CS1998 // missing await operators
        {
            /* Check if authorization header exists: */
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("no authorization header");
            }
            /* Parse header: */
            string username, password;
            try
            {
                var header = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var bytes = Convert.FromBase64String(header.Parameter);
                var credentials = Encoding.UTF8.GetString(bytes).Split(":", 2);
                username = credentials[0];
                password = credentials[1];
            }
            catch
            {
                return AuthenticateResult.Fail("invalid authorization header");
            }
            /* Check username & password: */
            if (username != _correctUsername || password != _correctPassword)
            {
                return AuthenticateResult.Fail("invalid username or password");
            }
            /* Success: */
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, username) };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }
        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Headers["WWW-Authenticate"] = "Basic realm=\"Presenter Area\"";
            await base.HandleChallengeAsync(properties);
        }

        private readonly string _correctUsername;
        private readonly string _correctPassword;
    }
}
