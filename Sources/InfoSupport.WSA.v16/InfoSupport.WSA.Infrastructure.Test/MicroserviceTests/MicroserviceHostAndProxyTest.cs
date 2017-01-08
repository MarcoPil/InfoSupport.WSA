using InfoSupport.WSA.Infrastructure;
using InfoSupport.WSA.Infrastructure.Test.dummies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class MicroserviceHostAndProxyTest
{
    [Fact]
    public void HandlerNotInInterfaceShouldNotBeTriggered()
    {
        var options = new BusOptions() { QueueName = "TestQueue01" };
        var serviceMock = new HalfServiceMock();
        using (var host = new MicroserviceHost<HalfServiceMock>(serviceMock, options))
        using (var proxy = new MicroserviceProxy("TestQueue01", options))
        {
            host.Open();

            var command = new SomeCommand() { SomeValue = "teststring" };
            Action action = () => proxy.Execute(command);

            Assert.Throws<MicroserviceException>(action);
            Assert.False(serviceMock.SomeCommandHandlerHasBeenCalled);
        }
    }

    [Fact]
    public void HandlerNotInInterfaceShouldNotBeTriggered2()
    {
        var options = new BusOptions() { QueueName = "TestQueue02" };
        var serviceMock = new HalfServiceMock();
        using (var host = new MicroserviceHost<HalfServiceMock>(serviceMock, options))
        using (var proxy = new MicroserviceProxy("TestQueue02", options))
        {
            host.Open();

            var command = new TestCommand();
            proxy.Execute(command);

            Thread.Sleep(500);
            Assert.True(serviceMock.TestCommandHandlerHasBeenCalled);
        }
    }
}