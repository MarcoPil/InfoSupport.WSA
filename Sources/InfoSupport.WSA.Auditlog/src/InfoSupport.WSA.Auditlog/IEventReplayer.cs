using InfoSupport.WSA.Logging.Model;
using System;

namespace InfoSupport.WSA.Logging
{
    public interface IEventReplayer : IDisposable
    {
        string ExchangeName { get;  set; }

        void Start();

        void ReplayLogEntry(LogEntry logEntry);

        void Stop();
    }
}