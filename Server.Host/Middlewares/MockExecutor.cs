using MockServer.Environment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using MockServer.Environment.Extensions;
using MockServer.Environment.Abstractions;
using ServerHost.Exceptions;
using ServerHost.Extensions;

namespace ServerHost.Middlewares
{
    public class MockExecutor
    {
        readonly IMatcher _matcher;
        readonly ILogger<MockExecutor> _logger;

        public MockExecutor(RequestDelegate next, IMatcher matcher, ILogger<MockExecutor> logger)
        {
            _matcher = matcher;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await LogRequest(context);
                var mockResponse = await MatchRequest(context);
                LogResponse(mockResponse);
                await WriteFinalResponse(context, mockResponse);
            }
            catch (NotMatchedException nmEx)
            {
                _logger.LogError(nmEx, nmEx.Message);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await HandleException(context, ex);
            }
        }

        private async Task<IMockResponse> MatchRequest(HttpContext context)
        {
            if (await _matcher.Match(context) is IResponseMatcher responseMatcher)
            {
                var response = await responseMatcher.Resolve();
                return response;
            }

            throw new NotMatchedException($"No matches found for request {context.Request.Method}: '{context.Request.GetPathAndQuery()}'");
        }

        private async Task WriteFinalResponse(HttpContext context, IMockResponse response)
        {
            context.AddResponseHeaders(response.Headers);
            context.SetResponseStatus(response.Status);

            await context.WriteBody(response.Body);
        }

        /// <summary>
        /// Log request
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <returns></returns>
        private async Task LogRequest(HttpContext context)
        {
            var request = JsonConvert.SerializeObject(new
            {
                Path = context.Request.GetPathAndQuery(),
                Headers = context.Request.Headers,
                Body = await context.GetRequestBody()
            });

            _logger.LogInformation($"Request => {request}");
        }


        /// <summary>
        /// Log response
        /// </summary>
        /// <param name="mockResponse">Final IMockResponse data</param>
        /// <returns></returns>
        private void LogResponse(IMockResponse mockResponse)
        {
            var response = JsonConvert.SerializeObject(new
            {
                Headers = mockResponse.Headers,
                Body = mockResponse.Body
            });

            _logger.LogInformation($"Response => {response}");
        }

        private async Task HandleException(HttpContext context, Exception ex)
        {
            if (ex is ResponseException exception)
            {
                context.SetResponseStatus(exception.StatusCode);

                if (exception.Message.StartsWith("{"))
                {
                    context.SetResponseContentType("application/json");
                }

                await context.WriteBody(exception.Message);
            }
            else
            {
                context.SetResponseStatus((int)HttpStatusCode.InternalServerError);

                if (!string.IsNullOrWhiteSpace(ex.Message))
                {
                    context.SetResponseContentType("application/json");

                    await context.WriteBody(JsonConvert.SerializeObject(new
                    {
                        error = ex.Message
                    }));
                }
            }
        }
    }
}
