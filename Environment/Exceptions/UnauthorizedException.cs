using System.Net;

namespace MockServer.Environment
{
    public class UnauthorizedException : ResponseException
    {
        public UnauthorizedException() : base((int)HttpStatusCode.Unauthorized)
        {
        }

        public UnauthorizedException(string message) : base((int)HttpStatusCode.Unauthorized, message)
        {
        }

        public UnauthorizedException(int statusCode, string message) : base(statusCode, message)
        {
        }
    }
}
