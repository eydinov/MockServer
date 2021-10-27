using MockServer.Environment;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using MockServer.Environment.Abstractions;
using ServerHost.Exceptions;
using ServerHost.Extensions;

namespace ServerHost.Services
{
    public class HeaderAuthHandler : IHeaderAuthorizationHandler
    {
        readonly ILogger<HeaderAuthHandler> _logger;

        public HeaderAuthHandler(ILogger<HeaderAuthHandler> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Entry point for x-api-key authorization handling
        /// </summary>
        /// <param name="context"></param>
        /// <param name="authData"></param>
        /// <returns></returns>
        public void Handle(HttpContext context, Authorization authData)
        {
            _logger.LogInformation("Checking claims...");
            if (!context.ValidateHeaders(authData.Claims))
                throw new UnauthorizedException(authData.UnauthorizedStatus == 0 ? StatusCodes.Status401Unauthorized : authData.UnauthorizedStatus, authData.UnauthorizedMessage);
        }
    }
}
