using InfoSupport.WSA.Infrastructure;
using InfoSupport.WSA.Infrastructure.Test.ExceptionsOverRabbitMqTests.dummies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class ExceptionsOverRabbitMQTest
{
    [Fact]
    public void MicroServiceThrowsException()
    {
        // Arrange
        var options = new BusOptions() { QueueName = "ExceptionThrowingTest01" };
        var serviceMock = new ExceptionThrowingService();
        using (var host = new MicroserviceHost<ExceptionThrowingService>(serviceMock, options))
        using (var proxy = new MicroserviceProxy("ExceptionThrowingTest01", options))
        {
            host.Open();

            // Act
            WorkCommand command = new WorkCommand { Name = "Marco" };
            Action action = () => proxy.Execute(command);

            // Assert
            Assert.Throws<NotImplementedException>(action);
        }
    }
}