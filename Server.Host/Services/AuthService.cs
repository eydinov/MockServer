using MockServer.Environment;
using Microsoft.AspNetCore.Http;
using System.Linq;
using MockServer.Environment.Abstractions;

namespace ServerHost.Services
{
    public class AuthService : IAuthorizationService
    {
        readonly IBasicAuthorizationHandler _basicHandler;
        readonly IBearerAuthorizationHandler _bearerHandler;
        readonly IApiKeyAuthorizationHandler _apiKeyHandler;

        public AuthService(IBasicAuthorizationHandler basicHandler, IBearerAuthorizationHandler bearerHandler, IApiKeyAuthorizationHandler apiKeyHandler)
        {
            _basicHandler = basicHandler;
            _bearerHandler = bearerHandler;
            _apiKeyHandler = apiKeyHandler;
        }

        /// <summary>
        /// Entry point for authorization handling
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="authData">Authorization data from configuration file</param>
        /// <returns></returns>
        public void Authorize(HttpContext context, Authorization[] authData)
        {
            if (authData == null)
                return;

            foreach (var auth in authData)
            {
                switch (auth?.Schema.ToLower())
                {
                    case "basic":
                        _basicHandler.Handle(context, auth);
                        break;
                    case "bearer":
                        _bearerHandler.Handle(context, auth);
                        break;
                    case "apikey":
                        _apiKeyHandler.Handle(context, auth);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
