using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MockServer.Environment.Abstractions
{
    public interface IResponseMatcher
    {
        MockOption Option { get; }
        Task<IMockResponse> Resolve();
        Task Validate(HttpContext context, MockOption mockConfig);
    }
}
