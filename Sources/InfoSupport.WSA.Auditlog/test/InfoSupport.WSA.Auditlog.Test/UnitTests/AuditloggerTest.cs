using InfoSupport.WSA.Logging.Test.Dummies;
using InfoSupport.WSA.Infrastructure;
using InfoSupport.WSA.Logging;
using InfoSupport.WSA.Logging.Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;


public class AuditloggerTest
{
    [Fact]
    public void Received_Events_Become_LogEntries()
    {
        NumberSentEvent sentEvent = new NumberSentEvent() { Number = 4 };
        LogEntry reveivedEntry = null;

        // Arrange Mock
        var mock = new Mock<ILogRepository>(MockBehavior.Strict);
        mock.Setup(repo => repo.AddEntry(It.IsAny<LogEntry>()))
            .Callback<LogEntry>(entry => { reveivedEntry = entry; });

        // Arrange Infrastructure
        var options = new BusOptions() { ExchangeName = "Auditlog01" };
        using (var logger = new AuditlogEventListener(mock.Object, options))
        using (var publisher = new EventPublisher(options))
        {
            logger.Start();

            // Act
            publisher.Publish(sentEvent);
            Thread.Sleep(500);

            // Assert
            Assert.Equal(sentEvent.Timestamp, reveivedEntry.Timestamp);
            Assert.Equal("Eventlogging.Test.NumberSentEvent", reveivedEntry.RoutingKey);
            Assert.Equal("InfoSupport.WSA.Logging.Test.Dummies.NumberSentEvent", reveivedEntry.EventType);
            var expextedJson = $"{{\"Number\":4,\"RoutingKey\":\"Eventlogging.Test.NumberSentEvent\",\"Timestamp\":{sentEvent.Timestamp}}}";
            Assert.Equal(expextedJson, reveivedEntry.EventJson);
        }
    }
}