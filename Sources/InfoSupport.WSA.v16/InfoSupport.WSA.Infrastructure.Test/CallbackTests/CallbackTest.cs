using InfoSupport.WSA.Infrastructure;
using InfoSupport.WSA.Infrastructure.Test.dummies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class CallbackTest
{
    [Fact]
    public void MicroServiceReturnsValue()
    {
        var options = new BusOptions() { QueueName = "CallbackTest01" };
        var serviceMock = new CallbackMock();
        using (var host = new MicroserviceHost<CallbackMock>(serviceMock, options))
        using (var proxy = new MicroserviceProxy("CallbackTest01", options))
        {
            host.Open();

            RequestCommand requestCommand = new RequestCommand { Name = "Marco" };
            TestResponse response = proxy.Execute<TestResponse>(requestCommand);

            Assert.Equal("Hello, Marco", response.Greeting);
        }
    }

    [Fact]
    public void MicroServiceResponseCorrelatesToRequest()
    {
        var options = new BusOptions() { QueueName = "CallbackTest02" };
        var serviceMock = new CallbackMock();
        using (var host = new MicroserviceHost<CallbackMock>(serviceMock, options))
        using (var proxy = new MicroserviceProxy("CallbackTest02", options))
        {
            host.Open();

            RequestCommand requestCommand = new RequestCommand { Name = "Marco" };
            SlowRequestCommand slowCommand = new SlowRequestCommand { Name = "Slow" };

            TestResponse slowResponse = proxy.Execute<TestResponse>(slowCommand);
            TestResponse response = proxy.Execute<TestResponse>(requestCommand);

            Assert.Equal("Hello, Marco", response.Greeting);
            Assert.Equal("Hello, Slow", slowResponse.Greeting);
        }
    }
}