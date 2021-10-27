using System.Collections.Generic;

namespace MockServer.Environment
{
    public class MockConfiguration : List<MockOption>
    {
    }

    public class MockOption
    {
        public string Name { get; set; }
        public Request Request { get; set; }
        public Response Response { get; set; }
    }
}
