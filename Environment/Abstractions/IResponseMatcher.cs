using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MockServer.Environment.Abstractions
{
    public interface IResponseMatcher
    {
        MockOption Option { get; }
        Task<IMockResponse> Resolve();
        Task Init(HttpContext context, MockOption mockConfig);
    }
}
