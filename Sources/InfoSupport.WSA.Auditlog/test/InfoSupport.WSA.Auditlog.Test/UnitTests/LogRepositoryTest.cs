using InfoSupport.WSA.Logging.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using InfoSupport.WSA.Logging.DAL;
using Microsoft.Extensions.DependencyInjection;
using InfoSupport.WSA.Logging;

public class LogRepositoryTest
{
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

    [Fact]
    public void AddEntry_to_LogRepository()
    {
        // Arrange
        var options = CreateNewContextOptions();
        var newEntry = new LogEntry();
        newEntry.RoutingKey = "Eventlogging.Test.NumberSentEvent";
        newEntry.EventType = "InfoSupport.WSA.Eventlogging.Test.NumberSentEvent";
        newEntry.Timestamp = 1234567;
        newEntry.EventJson = $"{{\"Number\":4,\"RoutingKey\":\"{newEntry.RoutingKey}\",\"Timestamp\":{newEntry.Timestamp}}}";

        ILogRepository target = new LogRepository(options);

        // Act
        target.AddEntry(newEntry);

        using (var context = new LoggerContext(options))
        {
            var entry = context.LogEntries.First(e => e.Timestamp == 1234567);

            Assert.True(entry.ID > 0);
            Assert.Equal(1234567, entry.Timestamp);
            Assert.Equal("Eventlogging.Test.NumberSentEvent", entry.RoutingKey);
            Assert.Equal("InfoSupport.WSA.Eventlogging.Test.NumberSentEvent", entry.EventType);
            Assert.Equal($"{{\"Number\":4,\"RoutingKey\":\"{newEntry.RoutingKey}\",\"Timestamp\":{newEntry.Timestamp}}}", entry.EventJson);
        }
    }

    [Fact]
    public void FindAllEntries()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new LoggerContext(options))
        {
            context.LogEntries.AddRange(
                new LogEntry { ID = 100, Timestamp = 123450, RoutingKey = "Test.SomeEvent", EventType = "SomeEvent", EventJson = "{\"Number\":0,\"RoutingKey\":\"Test.SomeEvent\",\"Timestamp\":123450}" },
                new LogEntry { ID = 101, Timestamp = 123451, RoutingKey = "Test.SomeEvent", EventType = "SomeEvent", EventJson = "{\"Number\":1,\"RoutingKey\":\"Test.SomeEvent\",\"Timestamp\":123451}" },
                new LogEntry { ID = 102, Timestamp = 123452, RoutingKey = "Test.SomeEvent", EventType = "SomeEvent", EventJson = "{\"Number\":2,\"RoutingKey\":\"Test.SomeEvent\",\"Timestamp\":123452}" }
            );
            context.SaveChanges();
        }

        ILogRepository target = new LogRepository(options);
        LogEntryCriteria criteria = new LogEntryCriteria();

        // Act
        var result = target.FindEntriesBy(criteria);

        // Assert
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public void FindAllEntries_UntilTimestamp()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new LoggerContext(options))
        {
            context.LogEntries.AddRange(
                new LogEntry { ID = 100, Timestamp = 123450, RoutingKey = "Test.SomeEvent", EventType = "SomeEvent", EventJson = "{\"Number\":0,\"RoutingKey\":\"Test.SomeEvent\",\"Timestamp\":123450}" },
                new LogEntry { ID = 101, Timestamp = 123451, RoutingKey = "Test.SomeEvent", EventType = "SomeEvent", EventJson = "{\"Number\":1,\"RoutingKey\":\"Test.SomeEvent\",\"Timestamp\":123451}" },
                new LogEntry { ID = 102, Timestamp = 123452, RoutingKey = "Test.SomeEvent", EventType = "SomeEvent", EventJson = "{\"Number\":2,\"RoutingKey\":\"Test.SomeEvent\",\"Timestamp\":123452}" }
            );
            context.SaveChanges();
        }

        ILogRepository target = new LogRepository(options);
        LogEntryCriteria criteria = new LogEntryCriteria() { ToTimestamp = 123451 };

        // Act
        var result = target.FindEntriesBy(criteria);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Equal(result.ElementAt(0).Timestamp, 123450);
        Assert.Equal(result.ElementAt(1).Timestamp, 123451);
    }

    [Fact]
    public void FindAllEntries_FromTimestamp()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new LoggerContext(options))
        {
            context.LogEntries.AddRange(
                new LogEntry { ID = 100, Timestamp = 123450, RoutingKey = "Test.SomeEvent", EventType = "SomeEvent", EventJson = "{\"Number\":0,\"RoutingKey\":\"Test.SomeEvent\",\"Timestamp\":123450}" },
                new LogEntry { ID = 101, Timestamp = 123451, RoutingKey = "Test.SomeEvent", EventType = "SomeEvent", EventJson = "{\"Number\":1,\"RoutingKey\":\"Test.SomeEvent\",\"Timestamp\":123451}" },
                new LogEntry { ID = 102, Timestamp = 123452, RoutingKey = "Test.SomeEvent", EventType = "SomeEvent", EventJson = "{\"Number\":2,\"RoutingKey\":\"Test.SomeEvent\",\"Timestamp\":123452}" }
            );
            context.SaveChanges();
        }

        ILogRepository target = new LogRepository(options);
        LogEntryCriteria criteria = new LogEntryCriteria() { FromTimestamp = 123451 };

        // Act
        var result = target.FindEntriesBy(criteria);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Equal(result.ElementAt(0).Timestamp, 123451);
        Assert.Equal(result.ElementAt(1).Timestamp, 123452);
    }

    [Fact]
    public void FindAllEntries_FromToTimestamp()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new LoggerContext(options))
        {
            context.LogEntries.AddRange(
                new LogEntry { ID = 100, Timestamp = 123450, RoutingKey = "Test.SomeEvent", EventType = "SomeEvent", EventJson = "{\"Number\":0,\"RoutingKey\":\"Test.SomeEvent\",\"Timestamp\":123450}" },
                new LogEntry { ID = 101, Timestamp = 123451, RoutingKey = "Test.SomeEvent", EventType = "SomeEvent", EventJson = "{\"Number\":1,\"RoutingKey\":\"Test.SomeEvent\",\"Timestamp\":123451}" },
                new LogEntry { ID = 102, Timestamp = 123452, RoutingKey = "Test.SomeEvent", EventType = "SomeEvent", EventJson = "{\"Number\":2,\"RoutingKey\":\"Test.SomeEvent\",\"Timestamp\":123452}" },
                new LogEntry { ID = 103, Timestamp = 123453, RoutingKey = "Test.SomeEvent", EventType = "SomeEvent", EventJson = "{\"Number\":3,\"RoutingKey\":\"Test.SomeEvent\",\"Timestamp\":123453}" }
            );
            context.SaveChanges();
        }

        ILogRepository target = new LogRepository(options);
        LogEntryCriteria criteria = new LogEntryCriteria() { FromTimestamp = 123451, ToTimestamp = 123452 };

        // Act
        var result = target.FindEntriesBy(criteria);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Equal(result.ElementAt(0).Timestamp, 123451);
        Assert.Equal(result.ElementAt(1).Timestamp, 123452);
    }

    [Fact]
    public void FindAllEntries_ByEventType()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new LoggerContext(options))
        {
            context.LogEntries.AddRange(
                new LogEntry { ID = 100, Timestamp = 123450, RoutingKey = "Eventlogging.Test.NumberSentEvent", EventType = "InfoSupport.WSA.Logging.Test.Dummies.NumberSentEvent", EventJson = "{\"Number\":0,\"RoutingKey\":\"Eventlogging.Test.NumberSentEvent\",\"Timestamp\":123450}" },
                new LogEntry { ID = 101, Timestamp = 123451, RoutingKey = "Eventlogging.Test.NameSentEvent", EventType = "InfoSupport.WSA.Logging.Test.Dummies.NameSentEvent", EventJson = "{\"Name\":\"Marco\",\"RoutingKey\":\"Eventlogging.Test.NameSentEvent\",\"Timestamp\":123451}" },
                new LogEntry { ID = 102, Timestamp = 123452, RoutingKey = "Eventlogging.Test.NumberSentEvent", EventType = "InfoSupport.WSA.Logging.Test.Dummies.NumberSentEvent", EventJson = "{\"Number\":2,\"RoutingKey\":\"Eventlogging.Test.NumberSentEvent\",\"Timestamp\":123452}" }
            );
            context.SaveChanges();
        }

        ILogRepository target = new LogRepository(options);
        LogEntryCriteria criteria = new LogEntryCriteria() { EventType = "InfoSupport.WSA.Logging.Test.Dummies.NumberSentEvent" };

        // Act
            var result = target.FindEntriesBy(criteria);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Equal(result.ElementAt(0).EventType, "InfoSupport.WSA.Logging.Test.Dummies.NumberSentEvent");
        Assert.Equal(result.ElementAt(1).EventType, "InfoSupport.WSA.Logging.Test.Dummies.NumberSentEvent");
    }

    [Fact]
    public void FindAllEntries_RoutingKey_FullMatch()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new LoggerContext(options))
        {
            context.LogEntries.AddRange(
                new LogEntry { ID = 100, Timestamp = 123450, RoutingKey = "Logging.Test.SomeEvent", EventType = "SomeEvent", EventJson = "{\"Number\":0,\"RoutingKey\":\"Logging.Test.SomeEvent\",\"Timestamp\":123450}" },
                new LogEntry { ID = 101, Timestamp = 123451, RoutingKey = "Logging.Test.OtherEvent", EventType = "OtherEvent", EventJson = "{\"Number\":1,\"RoutingKey\":\"Logging.Test.OtherEvent\",\"Timestamp\":123451}" },
                new LogEntry { ID = 102, Timestamp = 123452, RoutingKey = "Logging.Test.SomeEvent", EventType = "SomeEvent", EventJson = "{\"Number\":2,\"RoutingKey\":\"Logging.Test.SomeEvent\",\"Timestamp\":123452}" },
                new LogEntry { ID = 103, Timestamp = 123453, RoutingKey = "Logging.Test.AnotherEvent", EventType = "AnotherEvent", EventJson = "{\"Number\":3,\"RoutingKey\":\"Logging.Test.AnotherEvent\",\"Timestamp\":123453}" }
            );
            context.SaveChanges();
        }

        ILogRepository target = new LogRepository(options);
        LogEntryCriteria criteria = new LogEntryCriteria() { RoutingKeyExpression = "Logging.Test.SomeEvent" };

        // Act
        var result = target.FindEntriesBy(criteria);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Equal(result.ElementAt(0).RoutingKey, "Logging.Test.SomeEvent");
        Assert.Equal(result.ElementAt(1).RoutingKey, "Logging.Test.SomeEvent");
    }

    [Fact]
    public void FindAllEntries_RoutingKey_Astrix()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new LoggerContext(options))
        {
            context.LogEntries.AddRange(
                new LogEntry { ID = 100, Timestamp = 123450, RoutingKey = "Logging.Test.SomeEvent", EventType = "SomeEvent", EventJson = "{\"Number\":0,\"RoutingKey\":\"Logging.Test.SomeEvent\",\"Timestamp\":123450}" },
                new LogEntry { ID = 101, Timestamp = 123451, RoutingKey = "Logging.Test.OtherEvent", EventType = "OtherEvent", EventJson = "{\"Number\":1,\"RoutingKey\":\"Logging.Test.OtherEvent\",\"Timestamp\":123451}" },
                new LogEntry { ID = 102, Timestamp = 123452, RoutingKey = "Logging.Test.SomeEvent", EventType = "SomeEvent", EventJson = "{\"Number\":2,\"RoutingKey\":\"Logging.Test.SomeEvent\",\"Timestamp\":123452}" },
                new LogEntry { ID = 103, Timestamp = 123453, RoutingKey = "Logging.Test.AnotherEvent", EventType = "AnotherEvent", EventJson = "{\"Number\":3,\"RoutingKey\":\"Logging.Test.AnotherEvent\",\"Timestamp\":123453}" }
            );
            context.SaveChanges();
        }

        ILogRepository target = new LogRepository(options);
        LogEntryCriteria criteria = new LogEntryCriteria() { RoutingKeyExpression = "Logging.Test.*" };

        // Act
        var result = target.FindEntriesBy(criteria);

        // Assert
        Assert.Equal(4, result.Count());
        Assert.Equal(result.ElementAt(0).RoutingKey, "Logging.Test.SomeEvent");
        Assert.Equal(result.ElementAt(1).RoutingKey, "Logging.Test.OtherEvent");
        Assert.Equal(result.ElementAt(2).RoutingKey, "Logging.Test.SomeEvent");
        Assert.Equal(result.ElementAt(3).RoutingKey, "Logging.Test.AnotherEvent");
    }

    [Fact]
    public void FindAllEntries_RoutingKey_DotMatchesOnlyDot()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new LoggerContext(options))
        {
            context.LogEntries.AddRange(
                new LogEntry { ID = 100, Timestamp = 123450, RoutingKey = "Logging.Test.SomeEvent", EventType = "SomeEvent", EventJson = "{\"Number\":0,\"RoutingKey\":\"Logging.Test.SomeEvent\",\"Timestamp\":123450}" },
                new LogEntry { ID = 101, Timestamp = 123451, RoutingKey = "Logging.Test.OtherEvent", EventType = "OtherEvent", EventJson = "{\"Number\":1,\"RoutingKey\":\"Logging.Test.OtherEvent\",\"Timestamp\":123451}" },
                new LogEntry { ID = 102, Timestamp = 123452, RoutingKey = "Logging-Test.SomeEvent", EventType = "SomeEvent", EventJson = "{\"Number\":2,\"RoutingKey\":\"Logging.Test.SomeEvent\",\"Timestamp\":123452}" },
                new LogEntry { ID = 103, Timestamp = 123453, RoutingKey = "Logging.Test.AnotherEvent", EventType = "AnotherEvent", EventJson = "{\"Number\":3,\"RoutingKey\":\"Logging.Test.AnotherEvent\",\"Timestamp\":123453}" }
            );
            context.SaveChanges();
        }

        ILogRepository target = new LogRepository(options);
        LogEntryCriteria criteria = new LogEntryCriteria() { RoutingKeyExpression = "Logging.Test.*" };

        // Act
        var result = target.FindEntriesBy(criteria);

        // Assert
        Assert.Equal(3, result.Count());
        Assert.Equal(result.ElementAt(0).RoutingKey, "Logging.Test.SomeEvent");
        Assert.Equal(result.ElementAt(1).RoutingKey, "Logging.Test.OtherEvent");
        Assert.Equal(result.ElementAt(2).RoutingKey, "Logging.Test.AnotherEvent");
    }

    [Fact]
    public void FindAllEntries_RoutingKey_AstrixNotMatchesDot()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new LoggerContext(options))
        {
            context.LogEntries.AddRange(
                new LogEntry { ID = 100, Timestamp = 123450, RoutingKey = "Logging.Test.SomeEvent", EventType = "SomeEvent", EventJson = "{\"Number\":0,\"RoutingKey\":\"Logging.Test.SomeEvent\",\"Timestamp\":123450}" },
                new LogEntry { ID = 101, Timestamp = 123451, RoutingKey = "Logging.Test.OtherEvent", EventType = "OtherEvent", EventJson = "{\"Number\":1,\"RoutingKey\":\"Logging.Test.OtherEvent\",\"Timestamp\":123451}" },
                new LogEntry { ID = 102, Timestamp = 123452, RoutingKey = "Logging.Test.SomeEvent", EventType = "SomeEvent", EventJson = "{\"Number\":2,\"RoutingKey\":\"Logging.Test.SomeEvent\",\"Timestamp\":123452}" },
                new LogEntry { ID = 103, Timestamp = 123453, RoutingKey = "Logging.AnotherEvent", EventType = "AnotherEvent", EventJson = "{\"Number\":3,\"RoutingKey\":\"Logging.Test.AnotherEvent\",\"Timestamp\":123453}" }
            );
            context.SaveChanges();
        }

        ILogRepository target = new LogRepository(options);
        LogEntryCriteria criteria = new LogEntryCriteria() { RoutingKeyExpression = "Logging.*" };

        // Act
        var result = target.FindEntriesBy(criteria);

        // Assert
        Assert.Equal(1, result.Count());
        Assert.Equal(result.ElementAt(0).RoutingKey, "Logging.AnotherEvent");
    }

    [Fact]
    public void FindAllEntries_RoutingKey_HashAlsoMatchesDot()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new LoggerContext(options))
        {
            context.LogEntries.AddRange(
                new LogEntry { ID = 100, Timestamp = 123450, RoutingKey = "Logging.Test.SomeEvent", EventType = "SomeEvent", EventJson = "{\"Number\":0,\"RoutingKey\":\"Logging.Test.SomeEvent\",\"Timestamp\":123450}" },
                new LogEntry { ID = 101, Timestamp = 123451, RoutingKey = "Logging.Test.OtherEvent", EventType = "OtherEvent", EventJson = "{\"Number\":1,\"RoutingKey\":\"Logging.Test.OtherEvent\",\"Timestamp\":123451}" },
                new LogEntry { ID = 102, Timestamp = 123452, RoutingKey = "Logging.Test.SomeEvent", EventType = "SomeEvent", EventJson = "{\"Number\":2,\"RoutingKey\":\"Logging.Test.SomeEvent\",\"Timestamp\":123452}" },
                new LogEntry { ID = 103, Timestamp = 123453, RoutingKey = "Logging.AnotherEvent", EventType = "AnotherEvent", EventJson = "{\"Number\":3,\"RoutingKey\":\"Logging.Test.AnotherEvent\",\"Timestamp\":123453}" }
            );
            context.SaveChanges();
        }

        ILogRepository target = new LogRepository(options);
        LogEntryCriteria criteria = new LogEntryCriteria() { RoutingKeyExpression = "Logging.#" };

        // Act
        var result = target.FindEntriesBy(criteria);

        // Assert
        Assert.Equal(4, result.Count());
        Assert.Equal(result.ElementAt(0).RoutingKey, "Logging.Test.SomeEvent");
        Assert.Equal(result.ElementAt(1).RoutingKey, "Logging.Test.OtherEvent");
        Assert.Equal(result.ElementAt(2).RoutingKey, "Logging.Test.SomeEvent");
        Assert.Equal(result.ElementAt(3).RoutingKey, "Logging.AnotherEvent");
    }
}