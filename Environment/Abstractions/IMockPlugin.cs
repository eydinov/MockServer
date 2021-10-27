using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MockServer.Environment.Abstractions
{
    public interface IMockPlugin
    {
        Task<string> Execute();
        Task Validate(HttpContext context, MockOption option);
    }
}
