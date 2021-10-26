using System.Net;

namespace MockServer.Environment
{
    public class ForbiddenException : ResponseException
    {
        public ForbiddenException() : base((int)HttpStatusCode.Forbidden)
        {

        }
    }
}
