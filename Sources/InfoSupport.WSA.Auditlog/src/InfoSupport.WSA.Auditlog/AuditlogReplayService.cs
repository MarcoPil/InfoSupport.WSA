using System;
using InfoSupport.WSA.Infrastructure;
using InfoSupport.WSA.Logging;
using InfoSupport.WSA.Logging.Model;
using Newtonsoft.Json;
using System.Linq;
using System.Threading;

namespace InfoSupport.WSA.Logging
{
    public class AuditlogReplayService : IAuditlogReplayService
    {
        private readonly ILogRepository _logRepo;
        private readonly IEventReplayer _replayer;

        public AuditlogReplayService()
        {
        }
        public AuditlogReplayService(ILogRepository logRepo, IEventReplayer replayer)
        {
            _logRepo = logRepo;
            _replayer = replayer;
        }

        public void ReplayEvents(ReplayEventsCommand replayEventsCommand)
        {
            Console.WriteLine("Replay request received");

            var criteria = new LogEntryCriteria
            {
                FromTimestamp = replayEventsCommand.FromTimestamp,
                ToTimestamp = replayEventsCommand.ToTimestamp,
                EventType = replayEventsCommand.EventType,
                RoutingKeyExpression = replayEventsCommand.RoutingKeyExpression,
            };

            var entries = _logRepo.FindEntriesBy(criteria);

            Console.Write("Replaying {0} events ", entries.Count());

            _replayer.ExchangeName = replayEventsCommand.ExchangeName;
            _replayer.Start();
            foreach (var entry in entries)
            {
                Console.Write(".");
                _replayer.ReplayLogEntry(entry);
                Thread.Sleep(5);
            }
            Console.WriteLine();
            Console.WriteLine("Done replaying events.");
            _replayer.Stop();
        }
    }
}