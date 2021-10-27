using Microsoft.AspNetCore.Http;

namespace MockServer.Environment.Abstractions
{
    public interface IAuthorizationHandler
    {
        void Handle(HttpContext context, Authorization authData);
    }

    public interface IBasicAuthorizationHandler : IAuthorizationHandler
    {

    }

    public interface IBearerAuthorizationHandler : IAuthorizationHandler
    {

    }

    public interface IHeaderAuthorizationHandler : IAuthorizationHandler
    {

    }
}
