using MockServer.Environment;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using ServerHost.Models;
using System.Linq;
using System;
using MockServer.Environment.Extensions;
using MockServer.Environment.Abstractions;
using System.Text.RegularExpressions;
using System.Web;
using ServerHost.Extensions;

namespace ServerHost.Services
{
    public class ResponseMatcher : IResponseMatcher
    {
        readonly IAuthorizationService _authService;
        public MockOption Option { get; private set; }

        protected string PathAndQuery;

        public ResponseMatcher(MockOption option, IAuthorizationService authService)
        {
            _authService = authService;
            Option = option;
        }

        /// <summary>
        /// Handle possible preconfigured delay and send the final response
        /// </summary>
        /// <returns>MockResponse object</returns>
        public virtual async Task<IMockResponse> Resolve()
        {
            if (Option.Response.Delay > 0)
                await Task.Delay(Option.Response.Delay);

            return await SetResponse();
        }

        /// <summary>
        /// Create the response
        /// </summary>
        /// <returns>MockResponse object</returns>
        protected virtual async Task<IMockResponse> SetResponse()
        {
            var mockResponse = new MockResponse
            {
                Status = Option.Response.Status,
                Headers = Option.Response.Headers,
                Body = ReplaceWithRegex(await GetBody())
            };

            return await Task.FromResult(mockResponse);
        }

        protected virtual Task<string> GetBody()
        {
            var response = Option.Response.Body?.Props?.GetString("Body");

            if (Option.Response.Headers.Where(h => h.Key == "Content-Type" && h.Value.StartsWith(@"text/")).Any())
            {
                response = response.Replace(@"\n", Environment.NewLine);
            }

            return Task.FromResult(response);
        }

        protected virtual string ReplaceWithRegex(string replacement)
        {
            return !string.IsNullOrWhiteSpace(Option.Request.PathRegex)
                ? Regex.Replace(PathAndQuery, Option.Request.PathRegex, replacement)
                : replacement;
        }

        /// <summary>
        /// Validate the request.
        /// Every time the request is matching with response the first step to perform is validate the request against the context.
        /// Special logic can be implemented here (for example check response file exists, authorization is required, header contains etc...
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="option">Current MockOption object</param>
        /// <returns></returns>
        public virtual Task Init(HttpContext context, MockOption option)
        {
            AuthorizeRequest(context, option.Request.Authorization);

            PathAndQuery = HttpUtility.UrlDecode(context.GetRequestPathAndQuery());
            return Task.CompletedTask;
        }

        protected virtual Task AuthorizeRequest(HttpContext context, Authorization[] authorizations)
        {
            _authService.Authorize(context, authorizations);
            return Task.CompletedTask;
        }
    }
}
