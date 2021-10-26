using MockServer.Environment;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ServerHost.Models;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using MockServer.Environment.Extensions;
using MockServer.Environment.Abstractions;

namespace ServerHost.Services
{
    public class FileMatcher : ResponseMatcher
    {
        const string FILE_NAME_PROPS = "FileName";

        string _responseFile;
        string _regexResponse;
        readonly ILogger<FileMatcher> _logger;

        public FileMatcher(ILogger<FileMatcher> logger, MockOption option, IAuthorizationService authService) : base(option, authService)
        {
            _logger = logger;
            _responseFile = FileExtensions.GetFullPath(option.Response.Body.Props.GetString(FILE_NAME_PROPS));
            _regexResponse = _responseFile;
        }

        /// <summary>
        /// Set the response object
        /// </summary>
        /// <returns>Final response</returns>
        protected override async Task<IMockResponse> SetResponse()
        {
            var mockResponse = new MockResponse
            {
                Status = Option.Response.Status,
                Headers = Option.Response.Headers,
                Body = await GetBodyFromFile(_responseFile)
            };

            return mockResponse;
        }

        /// <summary>
        /// Validate the request.
        /// Every time the request is matching with response the first step to perform is validate the request against the context.
        /// Special logic can be implemented here (for example check response file exists, authorization is required, header contains etc...
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="option">Current MockOption object</param>
        /// <returns></returns>
        public override Task Validate(HttpContext context, MockOption option)
        {
            base.Validate(context, option);

            if (!string.IsNullOrWhiteSpace(option.Request.PathRegex))
            {
                _responseFile = Regex.Replace(HttpUtility.UrlDecode(context.Request.GetPathAndQuery()), option.Request.PathRegex, _regexResponse);
            }

            if (!File.Exists(_responseFile))
            {
                throw new NotFoundException();
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Returns body stored in file
        /// </summary>
        /// <param name="fileName">full UNC path to the file with response body</param>
        /// <returns></returns>
        private async Task<string> GetBodyFromFile(string fileName)
        {
            try
            {
                using var reader = File.OpenText(fileName);
                var bodyFromFile = await reader.ReadToEndAsync();

                return bodyFromFile;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }

        }

    }
}
