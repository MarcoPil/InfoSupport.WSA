using InfoSupport.WSA.Common;
using InfoSupport.WSA.Infrastructure;
using InfoSupport.WSA.Logging;
using InfoSupport.WSA.Logging.Model;
using InfoSupport.WSA.Logging.Test.Dummies;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class AuditlogServiceTest
{
    [Fact]
    public void ServiceCanReplayEvents()
    {
        // ExpectedOutcome
        LogEntry[] logEntries =
        {
            new LogEntry { Timestamp = 34643324461, RoutingKey = "Eventlogging.Test.NumberSentEvent", EventType = "InfoSupport.WSA.Logging.Test.Dummies.NumberSentEvent", EventJson = $"{{\"Number\":1,\"RoutingKey\":\"Eventlogging.Test.NumberSentEvent\",\"Timestamp\":34643324461}}" },
            new LogEntry { Timestamp = 34643324462, RoutingKey = "Eventlogging.Test.NumberSentEvent", EventType = "InfoSupport.WSA.Logging.Test.Dummies.NumberSentEvent", EventJson = $"{{\"Number\":2,\"RoutingKey\":\"Eventlogging.Test.NumberSentEvent\",\"Timestamp\":34643324462}}" },
            new LogEntry { Timestamp = 34643324463, RoutingKey = "Eventlogging.Test.NumberSentEvent", EventType = "InfoSupport.WSA.Logging.Test.Dummies.NumberSentEvent", EventJson = $"{{\"Number\":3,\"RoutingKey\":\"Eventlogging.Test.NumberSentEvent\",\"Timestamp\":34643324463}}" },
        };

        // Arrange Mocks
        var repoMock = new Mock<ILogRepository>(MockBehavior.Strict);
        repoMock.Setup(repo => repo.FindEntriesBy(It.IsAny<LogEntryCriteria>()))
            .Returns(logEntries);

        var reproducedEvents = new List<LogEntry>();
        var replayerMock = new Mock<IEventReplayer>(MockBehavior.Strict);
        replayerMock.Setup(replayer => replayer.ReplayLogEntry(It.IsAny<LogEntry>()))
                    .Callback<LogEntry>(entry => reproducedEvents.Add(entry));
        replayerMock.SetupSet(replayer => replayer.ExchangeName = "AuditlogServiceTestExchange01");
        replayerMock.Setup(replayer => replayer.Start());
        replayerMock.Setup(replayer => replayer.Stop());

        // Arrange
        var target = new AuditlogReplayService(repoMock.Object, replayerMock.Object);

        // Act
        ReplayEventsCommand replayCommand = new ReplayEventsCommand() { ExchangeName = "AuditlogServiceTestExchange01" };
        ReplayResult result = target.ReplayEvents(replayCommand);

        // Assert
        Assert.Equal(logEntries, reproducedEvents);
        replayerMock.VerifySet(replayer => replayer.ExchangeName = "AuditlogServiceTestExchange01");
        Assert.Equal(3, result.Count);
    }
}