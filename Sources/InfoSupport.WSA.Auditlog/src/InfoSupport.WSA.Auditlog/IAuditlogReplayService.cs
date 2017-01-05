using InfoSupport.WSA.Infrastructure;
using InfoSupport.WSA.Logging.Model;

namespace InfoSupport.WSA.Logging
{
    [Microservice]
    public interface IAuditlogReplayService
    {
        void ReplayEvents(ReplayEventsCommand replayEventsCommand);
    }
}