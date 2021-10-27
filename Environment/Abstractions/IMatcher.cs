using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MockServer.Environment.Abstractions
{
    public interface IMatcher
    {
        Task<IResponseMatcher> Match(HttpContext context);
    }
}
