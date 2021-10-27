using MockServer.Environment;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MockServer.Environment.Abstractions;

namespace Plugin.MockServer
{
    public class Dashboard : IMockPlugin
    {
        string _fileName;
        readonly MockConfiguration _mockOptions;
        readonly ILogger<Dashboard> _logger;
        readonly string rowBlock = "<tr><td><p class=\"label {0}_label\">{1}</p></td><td>{2}</td><td nowrap>{3}</td><td>{4}</td><td><p class=\"body_header\">Status Code: {5}</p><p class=\"body_header\">{6}</p><p class=\"body_text\">{7}</p></td></tr>";

        /// <summary>
        /// Constructor. All needed dependencies will be automatically resolved by DI
        /// </summary>
        /// <param name="logger">ILogger</param>
        /// <param name="configuration">Configuration</param>
        public Dashboard(IOptions<MockConfiguration> configuration, ILogger<Dashboard> logger)
        {
            _logger = logger;
            _mockOptions = configuration.Value;
        }

        /// <summary>
        /// Generate html page and return it as a response
        /// </summary>
        /// <returns></returns>
        public async Task<string> Execute()
        {
            try
            {
                var page = await File.ReadAllTextAsync(_fileName);
                StringBuilder sb_main = new StringBuilder();

                foreach (var option in _mockOptions)
                {
                    StringBuilder sb = new StringBuilder();
                    if (option.Response.Headers != null)
                    {
                        foreach (var header in option.Response.Headers)
                        {
                            sb.AppendLine($"{HttpUtility.HtmlEncode(header.Key)}: {HttpUtility.HtmlEncode(header.Value)}<br>");
                        }
                    }

                    var authSchemas = option.Request.Authorization?.Select(x => new string(HttpUtility.HtmlEncode(x.Schema)));

                    sb_main.AppendLine(string.Format(rowBlock,
                        option.Request.Method.ToLower(),
                        option.Request.Method, HttpUtility.HtmlEncode(option.Request.Path ?? option.Request.PathRegex),
                        HttpUtility.HtmlEncode(option.Name),
                        (authSchemas == null || !authSchemas.Any()) ? "-" : string.Join("<br>", authSchemas),
                        $"{option.Response.Status}",
                        sb.ToString(),
                        HttpUtility.HtmlEncode(GetBody(option.Response.Body))));
                }

                page = page.Replace("_options_", sb_main.ToString());
                return page;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Get body depending from configuration type
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        private string GetBody(Body body)
        {
            switch (body?.Type?.ToLower())
            {
                case "assembly":
                    return $"Response is handled by the plugin '{body.Props["Assembly"]}'";
                case "file":
                    return $"Response body is retrieved from the file '{body.Props["FileName"]}'";
                default:
                    return body?.Props["Body"] ?? "-";
            }
        }

        /// <summary>
        /// Validate if response file (index.html) exists or not
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="option">Mock option</param>
        /// <returns></returns>
        public Task Validate(HttpContext context, MockOption option)
        {
            _fileName = option.Response.Body.Props["Page"];

            if (_fileName.StartsWith("\\"))
            {
                var pathes = _fileName.Split('\\', StringSplitOptions.RemoveEmptyEntries).ToList();
                pathes.Insert(0, AppDomain.CurrentDomain.BaseDirectory);
                _fileName = Path.Combine(pathes.ToArray());
            }


            if (!File.Exists(_fileName))
            {
                throw new NotFoundException();
            }

            return Task.CompletedTask;
        }
    }
}
