using System;
using System.Collections.Generic;
using System.Text;

namespace WorkerServiceRabbitmq
{
    class Request
    {
        public string command { get; set; }
        public Test01 data { get; set; }
    }
}
