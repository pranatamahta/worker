using System;
using System.Collections.Generic;
using System.Text;

namespace WorkerServiceRabbitmq
{
    class Response
    {
        public string message { get; set; }
        public List<Test01> data { get; set; }
    }
}
