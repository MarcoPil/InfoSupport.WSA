using InfoSupport.WSA.Infrastructure;
using InfoSupport.WSA.Logging.Model;

namespace InfoSupport.WSA.Logging
{
    [Microservice]
    public interface IAuditlogReplayService
    {
        ReplayResult ReplayEvents(ReplayEventsCommand replayEventsCommand);
    }
}