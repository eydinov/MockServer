using System.Collections.Generic;

namespace MockServer.Environment
{
    public class Authorization : PropsBasedObject
    {
        public string Schema { get; set; }
        public int UnauthorizedStatus { get; set; }
        public string UnauthorizedMessage { get; set; }
        public IDictionary<string, string> Claims { get; set; }

    }
}
