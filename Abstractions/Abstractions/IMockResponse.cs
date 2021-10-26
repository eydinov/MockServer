using System.Collections.Generic;

namespace MockServer.Environment.Abstractions
{
    public interface IMockResponse
    {
        int Status { get; set; }
        Dictionary<string, string> Headers { get; set; }
        string Body { get; set; }
    }
}
