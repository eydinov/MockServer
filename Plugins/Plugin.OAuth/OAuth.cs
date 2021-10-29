using MockServer.Environment;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using MockServer.Environment.Extensions;
using MockServer.Environment.Abstractions;

namespace OAuth
{
    public class OAuth : IMockPlugin
    {
        const string ISSUER_PROPS = "iss";
        const string AUDIENCE_PROPS = "aud";
        const string EXPIRES_PROPS = "exp";
        const string BODY_PROPS = "Body";
        const string CLAIMS_PROPS = "Claims";


        string _response;

        readonly ILogger<OAuth> _logger;

        /// <summary>
        /// Constructor. All needed dependencies will be automatically resolved by DI
        /// </summary>
        /// <param name="logger">ILogger</param>
        public OAuth(ILogger<OAuth> logger)
        {
            _logger = logger;
        }

        public Task<string> Execute()
        {
            return Task.FromResult(_response);
        }

        public Task Init(HttpContext context, MockOption option)
        {
            CheckRequest(context, option);

            var exp_in = option.Response.Props.GetValueByKey<int>(EXPIRES_PROPS);
            var ref_ex_in = exp_in * 2;
            var access_token = BuildToken(exp_in, option.Response.Props.GetString(ISSUER_PROPS), option.Response.Props.GetString(AUDIENCE_PROPS), BuildClaims(option));
            var refresh_token = BuildToken(ref_ex_in);

            var props = new Dictionary<string, string>(option.Response.Props)
            {
                { "access_token", access_token },
                { "expires_in", exp_in.ToString() },
                { "refresh_token", refresh_token },
                { "refresh_expires_in", ref_ex_in.ToString() }
            };

            _response = props.SubstituteProps(option.Response.Body.Props.GetString(BODY_PROPS))
                .Replace("{token}", BuildToken());

            return Task.CompletedTask;
        }

        /// <summary>
        /// Build Claim's collection
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        private IList<Claim> BuildClaims(MockOption option)
        {
            try
            {
                var claims = option.Response.Body.Props.GetString(CLAIMS_PROPS);

                if (!string.IsNullOrWhiteSpace(claims))
                {
                    claims = option.Response.Props.SubstituteProps(claims);
                    var claimCollection = JsonConvert.DeserializeObject<Dictionary<string, string>>(claims);

                    var tokenClaims = new List<Claim>();
                    foreach (var claim in claimCollection)
                    {
                        tokenClaims.Add(new Claim(claim.Key, claim.Value));
                    }
                    return tokenClaims.Count > 0 ? tokenClaims : null;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new InternalException(ex.Message);
            }

        }

        /// <summary>
        /// Check if all data in form request is compared with what it is configured in Mock option request props
        /// </summary>
        /// <param name="context"></param>
        /// <param name="option"></param>
        private void CheckRequest(HttpContext context, MockOption option)
        {
            try
            {
                if (option.Request.Props is Dictionary<string, string> props)
                {
                    foreach (var prop in props)
                    {
                        if (!context.Request.HasFormContentType || context.Request.Form[prop.Key] != prop.Value)
                        {
                            if (!string.IsNullOrWhiteSpace(context.Request.Form[prop.Key]))
                            {
                                var error = new
                                {
                                    Error = $"invalid_{prop.Key}"
                                };

                                throw new BadRequestException(JsonConvert.SerializeObject(error));
                            }
                            else
                            {
                                var error = new
                                {
                                    Error = $"invalid_request",
                                    Error_description = $"MSIS9658: Received invalid OAuth request. The '{prop.Key}' parameter is missing in the request."
                                };

                                throw new BadRequestException(JsonConvert.SerializeObject(error));
                            }
                        }
                    }
                }
            }
            catch (BadRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new InternalException(ex.Message);
            }
        }

        /// <summary>
        /// Build Jwt token
        /// </summary>
        /// <param name="token_exp"></param>
        /// <param name="issues"></param>
        /// <param name="audience"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        private string BuildToken(int token_exp = 0, string issues = null, string audience = null, IList<Claim> claims = null)
        {
            try
            {
                var bytes = new byte[512];
                RandomNumberGenerator.Fill(bytes);
                var key = new SymmetricSecurityKey(bytes);
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
                var expiryDuration = token_exp;

                var token = new JwtSecurityToken(
                    issuer: issues,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddSeconds(expiryDuration),
                    signingCredentials: creds);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new InternalException(ex.Message);
            }
        }
    }
}
