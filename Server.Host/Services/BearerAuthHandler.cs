using MockServer.Environment;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using MockServer.Environment.Abstractions;
using ServerHost.Extensions;
using System.Collections.Generic;

namespace ServerHost.Services
{
    public class BearerAuthHandler : IBearerAuthorizationHandler
    {

        readonly JwtSecurityTokenHandler _tokenHandler;
        readonly ILogger<BearerAuthHandler> _logger;

        public BearerAuthHandler(ILogger<BearerAuthHandler> logger)
        {
            _logger = logger;
            _tokenHandler = new JwtSecurityTokenHandler();
        }

        /// <summary>
        /// Entry point for Bearer authorization handling
        /// </summary>
        /// <param name="context"></param>
        /// <param name="authData"></param>
        /// <returns></returns>
        public void Handle(HttpContext context, Authorization authData)
        {
            string authHeader = context.Request.Headers["Authorization"];

            if (authHeader != null && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Split(' ')[1]?.Trim();

                if (_tokenHandler.ReadJwtToken(token) is JwtSecurityToken jwtSecurityToken)
                {
                    _logger.LogInformation($"Bearer token '{token}' has been provided");

                    if (jwtSecurityToken.ValidTo < DateTime.UtcNow)
                    {
                        _logger.LogError("Bearer token has expired. Access forbidden");
                        throw new ForbiddenException();
                    }

                    if (!ValidateClaims(jwtSecurityToken.Claims, authData.Claims))
                        throw new ForbiddenException();
                }
            }
            else
            {
                _logger.LogError("No Bearer authorization header has been provided. Access denied");
                throw new UnauthorizedException(authData.UnauthorizedStatus == 0 ? StatusCodes.Status401Unauthorized : authData.UnauthorizedStatus, authData.UnauthorizedMessage);
            }
        }

        private bool ValidateClaims(IEnumerable<Claim> claims, IDictionary<string, string> claimsToCheck)
        {
            foreach (var claim in claimsToCheck)
            {
                bool res = claims.Any(x => x.Type == claim.Key && x.Value == claim.Value);
                if (!res)
                    return false;
            }

            return true;
        }
    }
}
