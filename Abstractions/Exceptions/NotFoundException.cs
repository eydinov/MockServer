using System.Net;

namespace MockServer.Environment
{
    public class NotFoundException : ResponseException
    {
        public NotFoundException() : base((int)HttpStatusCode.NotFound)
        {
        }
    }
}
