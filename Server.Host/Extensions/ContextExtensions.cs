using Microsoft.AspNetCore.Http;
using MockServer.Environment.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerHost.Extensions
{
    public static class ContextExtensions
    {
        public static void AddResponseHeaders(this HttpContext context, Dictionary<string, string> headers)
        {
            foreach (var header in headers)
            {
                context.Response.Headers.Add(header.Key, header.Value);
            }
        }

        public static void SetResponseStatus(this HttpContext context, int statusCode)
        {
            context.Response.StatusCode = statusCode;
        }

        public static void SetResponseContentType(this HttpContext context, string contentType)
        {
            context.Response.ContentType = contentType;
        }

        public static string GetRequestPathAndQuery(this HttpContext context)
        {
            return context.Request.GetPathAndQuery();
        }

        public static async Task WriteBody(this HttpContext context, string body)
        {
            if (!string.IsNullOrWhiteSpace(body))
            {
                await context.Response.WriteAsync(body);
            }
        }

        public static async Task<string> GetRequestBody(this HttpContext context)
        {
            string str = "";
            if (context.Request.Body != null)
            {
                context.Request.EnableBuffering();
                using Stream currentRequestBody = new MemoryStream();
                await context.Request.Body.CopyToAsync(currentRequestBody);

                using StreamReader reader = new(currentRequestBody);
                str = await reader.ReadToEndAsync();
                str = str.Replace(Environment.NewLine, "");
                context.Request.Body.Position = 0;
            }

            return str;
        }

        public static bool ValidateHeaders(this HttpContext context, Dictionary<string, string> claims)
        {
            foreach (var claim in claims)
            {
                bool res = context.Request.Headers.Any(x => x.Key == claim.Key && x.Value == claim.Value);
                if (!res)
                    return false;
            }

            return true;
        }
    }
}
