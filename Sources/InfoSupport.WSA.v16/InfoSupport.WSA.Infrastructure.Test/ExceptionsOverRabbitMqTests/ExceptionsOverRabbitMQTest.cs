using InfoSupport.WSA.Infrastructure;
using InfoSupport.WSA.Infrastructure.Test.ExceptionsOverRabbitMqTests.dummies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class ExceptionsOverRabbitMQTest
{
    //[Fact]
    //public void test()
    //{
    //    using (var host = new MicroserviceHost<ExceptionThrowingService>())
    //    using(var proxy = new MicroserviceProxy())
    //    {
    //        proxy.Execute()
    //    }
    //}

    [Fact]
    public void MicroServiceThrowsException()
    {
        // Arrange
        var options = new BusOptions() { QueueName = "ExceptionThrowingTest01" };
        using (var host = new MicroserviceHost<ExceptionThrowingService>(options))
        using (var proxy = new MicroserviceProxy(options))
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