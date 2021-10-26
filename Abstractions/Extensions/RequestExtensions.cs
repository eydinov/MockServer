using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MockServer.Environment.Extensions
{
    public static class RequestExtensions
    {
        public static string GetPathAndQuery(this HttpRequest request)
        {
            return request.GetEncodedPathAndQuery().TrimEnd("?=".ToCharArray());
        }
    }
}
