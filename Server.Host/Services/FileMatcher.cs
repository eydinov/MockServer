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
using ServerHost.Extensions;

namespace ServerHost.Services
{
    public class FileMatcher : ResponseMatcher
    {
        const string FILE_NAME_PROPS = "FileName";

        string _responseFile;
        readonly string _regexResponse;
        readonly ILogger<FileMatcher> _logger;

        public FileMatcher(ILogger<FileMatcher> logger, MockOption option, IAuthorizationService authService) : base(option, authService)
        {
            _logger = logger;
            _responseFile = FileExtensions.GetFullPath(option.Response.Body.Props.GetString(FILE_NAME_PROPS));
            _regexResponse = _responseFile;
        }

        protected override async Task<string> GetBody()
        {
            try
            {
                using var reader = File.OpenText(_responseFile);
                var bodyFromFile = await reader.ReadToEndAsync();

                return ReplaceWithRegex(bodyFromFile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public override Task Validate(HttpContext context, MockOption option)
        {
            base.Validate(context, option);

            _responseFile = ReplaceWithRegex(_regexResponse);

            if (!File.Exists(_responseFile))
            {
                throw new NotFoundException();
            }

            return Task.CompletedTask;
        }
    }
}
