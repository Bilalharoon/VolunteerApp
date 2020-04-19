using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleAPI.Helpers
{
    public class HttpResponseException:Exception
    {
        public HttpResponseException(string value)
        {
            Value = value;
        }
        public int Status { get; set; } = 400;
        public string Value { get; set; }
    }
}
