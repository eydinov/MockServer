using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerHost.Exceptions
{
    public class ApiKeyNotFoundException : Exception
    {
        public ApiKeyNotFoundException(string message = null) : base(message)
        {
        }
    }
}
