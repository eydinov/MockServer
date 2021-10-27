using System.Net;

namespace MockServer.Environment
{
    public class BadRequestException : ResponseException
    {

        public BadRequestException() : base((int)HttpStatusCode.BadRequest)
        {
        }

        public BadRequestException(string message) : base((int)HttpStatusCode.BadRequest, message)
        {
        }
    }
}
