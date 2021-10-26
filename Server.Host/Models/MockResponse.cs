using MockServer.Environment;
using MockServer.Environment.Abstractions;
using System.Collections.Generic;

namespace ServerHost.Models
{
    public class MockResponse : IMockResponse
    {
        public int Status { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string Body { get; set; }
    }
}
