using System.Collections.Generic;

namespace MockServer.Environment
{
    public class Response : PropsBasedObject
    {
        public int Status { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public Body Body { get; set; }
        public int Delay { get; set; }

    }
}
