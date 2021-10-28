using MockServer.Environment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServerHost.Models;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using MockServer.Environment.Extensions;
using MockServer.Environment.Abstractions;
using ServerHost.Extensions;

namespace ServerHost.Services
{
    public class MatcherService : IMatcher
    {
        readonly MockConfiguration _configuration;
        readonly ConcurrentDictionary<IMatchKey, IResponseMatcher> dict = new();
        readonly IServiceProvider _provider;


        public MatcherService(IServiceProvider provider, IOptions<MockConfiguration> configuration)
        {
            _configuration = configuration?.Value;
            _provider = provider;

            foreach (var option in _configuration)
            {
                var key = !string.IsNullOrWhiteSpace(option.Request.PathRegex)
                    ? new MatchKey
                    {
                        Method = option.Request.Method,
                        Regex = new(option.Request.PathRegex, RegexOptions.Compiled),
                    }
                    : new MatchKey
                    {
                        Method = option.Request.Method,
                        Path = option.Request.Path
                    };

                dict.TryAdd(key, GetMatcher(option));
            }
        }

        /// <summary>
        /// Matching the request
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <returns>IResponseMatcher object</returns>
        public async Task<IResponseMatcher> Match(HttpContext context)
        {
            var method = context.Request.Method;
            var path = HttpUtility.UrlDecode(context.GetRequestPathAndQuery());

            var res = dict.FirstOrDefault(x => x.Key.IsMatch(method, path));

            if (res.Value is IResponseMatcher matcher)
            {
                await matcher?.Validate(context, matcher.Option);
                return matcher;
            }
            return null;
        }

        /// <summary>
        /// Get proper matcher
        /// </summary>
        /// <param name="option">MockOption to be matched</param>
        /// <returns></returns>
        private IResponseMatcher GetMatcher(MockOption option)
        {
            IResponseMatcher response = option.Response.Body?.Type?.ToLower() switch
            {
                "assembly" => ActivatorUtilities.CreateInstance<AssemblyMatcher>(_provider, option),
                "file" => ActivatorUtilities.CreateInstance<FileMatcher>(_provider, option),
                _ => ActivatorUtilities.CreateInstance<ResponseMatcher>(_provider, option)
            };

            return response;
        }

    }
}
