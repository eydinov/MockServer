using System;

namespace MockServer.Environment
{
    public class ResponseException : Exception
    {
        public int StatusCode { get; private set; }

        public ResponseException(int statusCode) : base("")
        {
            StatusCode = statusCode;
        }
        public ResponseException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
