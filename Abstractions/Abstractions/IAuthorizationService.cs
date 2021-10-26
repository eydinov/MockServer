using Microsoft.AspNetCore.Http;

namespace MockServer.Environment.Abstractions
{
    public interface IAuthorizationService
    {
        void Authorize(HttpContext context, Authorization[] authData);
    }
}
