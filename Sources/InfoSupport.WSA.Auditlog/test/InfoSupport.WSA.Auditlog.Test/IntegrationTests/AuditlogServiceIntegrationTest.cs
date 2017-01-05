using InfoSupport.WSA.Infrastructure;
using InfoSupport.WSA.Logging;
using InfoSupport.WSA.Logging.DAL;
using InfoSupport.WSA.Logging.Model;
using InfoSupport.WSA.Logging.Test.IntegrationTests.Dummies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class AuditlogServiceIntegrationTest
{
    [Fact]
    public void AuditlogLogsEvents()
    {
        // Arrange
        DbContextOptions<LoggerContext> dbOptions = CreateNewContextOptions();
        BusOptions busOptions = new BusOptions() { ExchangeName = "AuditlogServiceIntegrationTestEx01" };

        var auditlogService = new AuditlogReplayService(
                                    new LogRepository(dbOptions),
                                    new EventReplayer(busOptions));

        // Act
        using (var auditlog = new AuditlogEventListener(new LogRepository(dbOptions), busOptions))
        {
            auditlog.Start();

            using (var pub1 = new EventPublisher(busOptions))
            using (var pub2 = new EventPublisher(busOptions))
            {
                pub1.Publish(new Number1Event() { Number = 1 });
                pub2.Publish(new Number2Event() { Number = 2 });
                pub1.Publish(new Name1Event() { Name = "pub1" });
                pub2.Publish(new Name2Event() { Name = "pub2" });
            }

            Thread.Sleep(1000);
        }

        // Assert
        using (var context = new LoggerContext(dbOptions))
        {
            IEnumerable<LogEntry> result = context.LogEntries;
            Assert.Equal(4, result.Count());
            Assert.Equal("InfoSupport.WSA.Logging.Test.IntegrationTests.Dummies.Number1Event", result.ElementAt(0).EventType);
            Assert.Equal("InfoSupport.WSA.Logging.Test.IntegrationTests.Dummies.Number2Event", result.ElementAt(1).EventType);
            Assert.Equal("InfoSupport.WSA.Logging.Test.IntegrationTests.Dummies.Name1Event", result.ElementAt(2).EventType);
            Assert.Equal("InfoSupport.WSA.Logging.Test.IntegrationTests.Dummies.Name2Event", result.ElementAt(3).EventType);
        }
    }

    [Fact]
    public void AuditlogReplaysEvents()
    {
        // Arrange Mocks
        DbContextOptions<LoggerContext> dbOptions = CreateNewContextOptions();
        FillAuditLogDatabaseWithEvents(dbOptions);

        BusOptions busOptions = new BusOptions() { ExchangeName = "AuditlogServiceIntegrationTestEx01", QueueName = "EndToEndTestQ01" };

        var reproducedEvents = new List<LogEntry>();
        var replayerMock = new Mock<IEventReplayer>(MockBehavior.Strict);
        replayerMock.Setup(replayer => replayer.ReplayLogEntry(It.IsAny<LogEntry>()))
                    .Callback<LogEntry>(entry => reproducedEvents.Add(entry));
        replayerMock.SetupSet(replayer => replayer.ExchangeName = "AuditlogReplaysEvents");
        replayerMock.Setup(replayer => replayer.Start());
        replayerMock.Setup(replayer => replayer.Stop());

        // Arrange
        var auditlogService = new AuditlogReplayService(
                                    new LogRepository(dbOptions),
                                    replayerMock.Object);

        using (var host = new MicroserviceHost<AuditlogReplayService>(auditlogService, busOptions))
        using (var proxy = new MicroserviceProxy(busOptions))
        {
            host.Open();

            var replayCommand = new ReplayEventsCommand() { ExchangeName = "AuditlogReplaysEvents" };
            // Act
            proxy.Execute(replayCommand);

            Thread.Sleep(100);

            // Assert
            replayerMock.Verify(replayer => replayer.ReplayLogEntry(It.IsAny<LogEntry>()), Times.Exactly(4));
            Assert.Equal(4, reproducedEvents.Count());
            Assert.Equal("IntegrationTest.Pub1.Number1Event", reproducedEvents[0].RoutingKey);
            Assert.Equal("IntegrationTest.Pub2.Number2Event", reproducedEvents[1].RoutingKey);
            Assert.Equal("IntegrationTest.Pub1.Name1Event", reproducedEvents[2].RoutingKey);
            Assert.Equal("IntegrationTest.Pub2.Name2Event", reproducedEvents[3].RoutingKey);
        }
    }

    [Fact]
    public void ReplayerReplaysEvents()
    {
        BusOptions busOptions = new BusOptions() { ExchangeName = "EndToEndTestEx01" };

        var replayBusOptions = new BusOptions() { ExchangeName = "ReplayerReplaysEvents" };
        using (var eventListener = new ReplayEventReceiverMock(replayBusOptions))
        using (var replayer = new EventReplayer(busOptions))
        {
            eventListener.Open();

            Thread.Sleep(1000);

            replayer.ExchangeName = "ReplayerReplaysEvents";
            replayer.Start();
            foreach (var logEntry in LogEntries)
            {
                replayer.ReplayLogEntry(logEntry);
            }
            replayer.Stop();

            Thread.Sleep(1000);

            Assert.Equal(4, eventListener.ReceivedEvents.Count());
            Assert.Equal("IntegrationTest.Pub1.Number1Event", eventListener.ReceivedEvents[0].RoutingKey);
            Assert.Equal("IntegrationTest.Pub2.Number2Event", eventListener.ReceivedEvents[1].RoutingKey);
            Assert.Equal("IntegrationTest.Pub1.Name1Event", eventListener.ReceivedEvents[2].RoutingKey);
            Assert.Equal("IntegrationTest.Pub2.Name2Event", eventListener.ReceivedEvents[3].RoutingKey);
        }
    }

    [Fact]
    public void EndToEndTest()
    {
        DbContextOptions<LoggerContext> dbOptions = CreateNewContextOptions();
        BusOptions busOptions = new BusOptions() { ExchangeName = "EndToEndTestEx01", QueueName = "EndToEndTestQ01" };

        // Start the Auditlog and ReplayService, using an in-memory database
        var auditlogReplayService = new AuditlogReplayService(
                                        new LogRepository(dbOptions),
                                        new EventReplayer(busOptions));
        using (var auditlog = new AuditlogEventListener(new LogRepository(dbOptions), busOptions))
        using(var replayHost = new MicroserviceHost<AuditlogReplayService>(auditlogReplayService, busOptions))
        {
            auditlog.Start();
            replayHost.Open();

            // Publish four different events from two different sources
            using (var pub1 = new EventPublisher(busOptions))
            using (var pub2 = new EventPublisher(busOptions))
            {
                pub1.Publish(new Number1Event() { Number = 1 });
                pub2.Publish(new Number2Event() { Number = 2 });
                pub1.Publish(new Name1Event() { Name = "pub1" });
                pub2.Publish(new Name2Event() { Name = "pub2" });
            }

            // wait until all events have been logged
            Thread.Sleep(100);

            // Start replaying events
            var replayBusOptions = new BusOptions() { ExchangeName = "EndToEndTestReplayExchange" };
            using (var eventListener = new ReplayEventReceiverMock(replayBusOptions))
            using (var proxy = new MicroserviceProxy(busOptions))
            {
                eventListener.Open();

                var replayCommand = new ReplayEventsCommand() { ExchangeName = "EndToEndTestReplayExchange" };
                proxy.Execute(replayCommand);

                // wait until all events have been replayed
                Thread.Sleep(100);

                Assert.Equal(4, eventListener.ReceivedEvents.Count());
                Assert.Equal("IntegrationTest.Pub1.Number1Event", eventListener.ReceivedEvents[0].RoutingKey);
                Assert.Equal("IntegrationTest.Pub2.Number2Event", eventListener.ReceivedEvents[1].RoutingKey);
                Assert.Equal("IntegrationTest.Pub1.Name1Event", eventListener.ReceivedEvents[2].RoutingKey);
                Assert.Equal("IntegrationTest.Pub2.Name2Event", eventListener.ReceivedEvents[3].RoutingKey);
                Assert.Equal("InfoSupport.WSA.Logging.Test.IntegrationTests.Dummies.Number1Event", eventListener.ReceivedEvents[0].GetType().FullName);
                Assert.Equal("InfoSupport.WSA.Logging.Test.IntegrationTests.Dummies.Number2Event", eventListener.ReceivedEvents[1].GetType().FullName);
                Assert.Equal("InfoSupport.WSA.Logging.Test.IntegrationTests.Dummies.Name1Event", eventListener.ReceivedEvents[2].GetType().FullName);
                Assert.Equal("InfoSupport.WSA.Logging.Test.IntegrationTests.Dummies.Name2Event", eventListener.ReceivedEvents[3].GetType().FullName);
            }
        }
    }

    private static DbContextOptions<LoggerContext> CreateNewContextOptions()
    {
        // Create a fresh service provider, and therefore a fresh 
        // InMemory database instance.
        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

        // Create a new options instance telling the context to use an
        // InMemory database and the new service provider.
        var builder = new DbContextOptionsBuilder<LoggerContext>();
        builder.UseInMemoryDatabase()
               .UseInternalServiceProvider(serviceProvider);

        return builder.Options;
    }

    private static LogEntry[] LogEntries = new LogEntry[]
    {
        new LogEntry
        {
            EventType = "InfoSupport.WSA.Logging.Test.IntegrationTests.Dummies.Number1Event",
            RoutingKey = "IntegrationTest.Pub1.Number1Event",
            Timestamp = 1234561,
            EventJson = "{\"Number\":1,\"RoutingKey\":\"IntegrationTest.Pub1.Number1Event\",\"Timestamp\":1234561}"
        },
       new LogEntry
        {
            EventType = "InfoSupport.WSA.Logging.Test.IntegrationTests.Dummies.Number2Event",
            RoutingKey = "IntegrationTest.Pub2.Number2Event",
            Timestamp = 1234562,
            EventJson = "{\"Number\":2,\"RoutingKey\":\"IntegrationTest.Pub2.Number2Event\",\"Timestamp\":1234562}"
        },
       new LogEntry
       {
            EventType = "InfoSupport.WSA.Logging.Test.IntegrationTests.Dummies.Name1Event",
            RoutingKey = "IntegrationTest.Pub1.Name1Event",
            Timestamp = 1234563,
            EventJson = "{\"Name\":\"Jan\",\"RoutingKey\":\"IntegrationTest.Pub1.Name1Event\",\"Timestamp\":1234563}"
        },
       new LogEntry
       {
            EventType = "InfoSupport.WSA.Logging.Test.IntegrationTests.Dummies.Name2Event",
            RoutingKey = "IntegrationTest.Pub2.Name2Event",
            Timestamp = 1234564,
            EventJson = "{\"Name\":\"Fatima\",\"RoutingKey\":\"IntegrationTest.Pub2.Name2Event\",\"Timestamp\":1234564}"
        }
    };

    private static void FillAuditLogDatabaseWithEvents(DbContextOptions<LoggerContext> dbOptions)
    {
        using (var context = new LoggerContext(dbOptions))
        {
            context.LogEntries.AddRange(LogEntries);
            context.SaveChanges();
        }
    }

}