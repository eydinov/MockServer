using MockServer.Environment;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using MockServer.Environment.Abstractions;

namespace ServerHost.Services
{
    public class BasicAuthHandler : IBasicAuthorizationHandler
    {
        readonly ILogger<BasicAuthHandler> _logger;

        /// <summary>
        /// Constructor. All dependencies will be automatically resolved by DI
        /// </summary>
        public BasicAuthHandler(ILogger<BasicAuthHandler> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Entry point for basic authorization handling
        /// </summary>
        /// <param name="context"></param>
        /// <param name="authData"></param>
        /// <returns></returns>
        public void Handle(HttpContext context, Authorization authData)
        {
            try
            {
                string authHeader = context.Request.Headers["Authorization"];

                if (authHeader != null && authHeader.StartsWith("Basic "))
                {
                    _logger.LogInformation($"Basic authorization header '{authHeader}' has been provided");

                    var encodedUsernamePassword = authHeader.Split(' ')[1]?.Trim();
                    var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword)).Split(':');
                    var username = decodedUsernamePassword[0];
                    var password = decodedUsernamePassword[1];
                    var res = username.Equals(authData.Claims["UserName"], StringComparison.InvariantCultureIgnoreCase) && password.Equals(authData.Claims["Password"]);

                    if (!res)
                        throw new Exception("Wrong UserName or Password");

                    return;
                }
                else
                {
                    context.Response.Headers["WWW-Authenticate"] = "Basic";
                    context.Response.Headers["WWW-Authenticate"] += $" realm=\"{authData.Claims["Realm"]}\"";

                    throw new Exception("No Basic authorization header has been provided. Access denied");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new UnauthorizedException(authData.UnauthorizedStatus == 0 ? StatusCodes.Status401Unauthorized : authData.UnauthorizedStatus, authData.UnauthorizedMessage);
            }
        }
    }
}
