using System.Net;

namespace MockServer.Environment
{
    public class InternalException : ResponseException
    {
        public InternalException() : base((int)HttpStatusCode.InternalServerError)
        {
        }

        public InternalException(string message) : base((int)HttpStatusCode.InternalServerError, message)
        {
        }
    }
}
