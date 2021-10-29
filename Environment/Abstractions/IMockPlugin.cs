using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MockServer.Environment.Abstractions
{
    public interface IMockPlugin
    {
        Task<string> Execute();
        Task Init(HttpContext context, MockOption option);
    }
}
