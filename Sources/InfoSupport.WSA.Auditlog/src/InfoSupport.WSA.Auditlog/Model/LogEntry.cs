using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoSupport.WSA.Logging.Model
{
    public class LogEntry
    {
        public long ID { get; set; }
        public long Timestamp { get; set; }
        public string RoutingKey { get; set; }
        public string EventType { get; set; }
        public string EventJson { get; set; }
    }
}
