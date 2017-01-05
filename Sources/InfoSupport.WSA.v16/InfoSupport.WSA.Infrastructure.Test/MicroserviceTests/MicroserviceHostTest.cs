using InfoSupport.WSA.Infrastructure;
using InfoSupport.WSA.Infrastructure.Test.dummies;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class MicroserviceHostTest
{
    [Fact]
    public void MicroserviceHostFindsHandlers()
    {
        using (var host = new MicroserviceHost<SomeMicroserviceMock>())
        {
            Assert.Equal(2, host.ServiceModel.Handlers.Count());
            Assert.True(host.ServiceModel.Handlers.Contains("InfoSupport.WSA.Infrastructure.Test.dummies.TestCommand"), "TestCommand is not recognized");
            Assert.True(host.ServiceModel.Handlers.Contains("InfoSupport.WSA.Infrastructure.Test.dummies.SomeCommand"), "SomeCommand is not recognized");
        }
    }

    [Fact]
    public void MicroserviceHostFindsHandlersInInstance()
    {
        var serviceMock = new SomeMicroserviceMock();
        using (var host = new MicroserviceHost<SomeMicroserviceMock>(serviceMock))
        {
            Assert.Equal(2, host.ServiceModel.Handlers.Count());
            Assert.True(host.ServiceModel.Handlers.Contains("InfoSupport.WSA.Infrastructure.Test.dummies.TestCommand"), "TestCommand is not recognized");
            Assert.True(host.ServiceModel.Handlers.Contains("InfoSupport.WSA.Infrastructure.Test.dummies.SomeCommand"), "SomeCommand is not recognized");
        }
    }

    [Fact]
    public void MicroserviceHostFindsQueueNameInAttribute()
    {
        var serviceMock = new SomeMicroserviceMock();
        using (var host = new MicroserviceHost<SomeMicroserviceMock>(serviceMock))
        {
            Assert.Equal(1, host.ServiceModel.QueueNames.Count());
            Assert.True(host.ServiceModel.QueueNames.Contains("microserviceQueue"));
        }
    }


    [Fact]
    public void MicroserviceHostFindsQueueNameInBusOptions()
    {
        var options = new BusOptions { QueueName = "ThrowAwayName" };
        using (var host = new MicroserviceHost<OtherMicroserviceMock>(options))
        {
            Assert.Equal(1, host.ServiceModel.QueueNames.Count());
            Assert.True(host.ServiceModel.QueueNames.Contains("ThrowAwayName"));
        }
    }

    [Fact]
    public void MicroserviceHostReceivesCommands()
    {
        var serviceMock = new SomeMicroserviceMock();
        using (var host = new MicroserviceHost<SomeMicroserviceMock>(serviceMock))
        {
            var options = new BusOptions() { QueueName = "microserviceQueue" };

            host.Open();

            var command = new SomeCommand() { SomeValue = "teststring" };
            SendMessage(command, options);
            serviceMock.ReceivedFlag.WaitOne(500);

            Assert.True(serviceMock.SomeCommandHandlerHasBeenCalled);
            Assert.False(serviceMock.TestCommandHandlerHasBeenCalled);
        }
    }

    [Fact]
    public void MicroserviceHostSilentlyIgnoresUnknownCommands()
    {
        var serviceMock = new SomeMicroserviceMock();
        using (var host = new MicroserviceHost<SomeMicroserviceMock>(serviceMock))
        {
            var options = new BusOptions() { QueueName = "microserviceQueue" };
            var command = new KeyValuePair<string, int>("Test", 42);
            SendMessage(command, options);
            serviceMock.ReceivedFlag.WaitOne(500);

            Assert.False(serviceMock.SomeCommandHandlerHasBeenCalled);
            Assert.False(serviceMock.TestCommandHandlerHasBeenCalled);
        }
    }

    private void SendMessage(object command, BusOptions busOptions)
    {
        var factory = new ConnectionFactory()
        {
            HostName = busOptions.HostName,
            Port = busOptions.Port,
            UserName = busOptions.UserName,
            Password = busOptions.Password,
        };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: busOptions.QueueName,
                                 durable: false, exclusive: false, autoDelete: false, arguments: null);

            // set metadata
            var props = channel.CreateBasicProperties();
            props.Type = command.GetType().FullName;
            // set payload
            string message = JsonConvert.SerializeObject(command);
            var buffer = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "",
                                 routingKey: busOptions.QueueName,
                                 basicProperties: props,
                                 body: buffer);
        }
    }

    [Fact]
    public void MicroserviceHostWithoutMicroServiceAttributeFails()
    {
        MicroserviceConfigurationException ex =
            Assert.Throws<MicroserviceConfigurationException>(() =>
           {
               using (var host = new MicroserviceHost<BusOptions>())
               {
               }
           });
        Assert.Equal("No [MicroService] interfaces have been found.", ex.Message);
    }

    [Fact]
    public void MicroserviceHostWithoutHandlersFails()
    {
        var serviceMock = new EmptyServiceMock();
        MicroserviceConfigurationException ex =
            Assert.Throws<MicroserviceConfigurationException>(() =>
            {
                using (var host = new MicroserviceHost<EmptyServiceMock>(serviceMock))
                {
                }
            });
        Assert.Equal("No Handlers can be found in the Microservice interface.", ex.Message);
    }

    [Fact]
    public void MicroserviceHostWithoutConfiguredQueueNameFails()
    {
        var serviceMock = new ServiceWithoutQueueMock();
        MicroserviceConfigurationException ex =
            Assert.Throws<MicroserviceConfigurationException>(() =>
            {
                using (var host = new MicroserviceHost<ServiceWithoutQueueMock>(serviceMock))
                {
                    host.Open();
                }
            });
        Assert.Equal("No queue name is configured in the MicroserviceAtrribute on any Microservice interface nor in the Busoptions.", ex.Message);
    }

    [Fact]
    public void MicroserviceHostFailsIfRabbitMQIsNotReachable()
    {
        var serviceMock = new SomeMicroserviceMock();
        var options = new BusOptions { HostName = "NonExistingName" };
        MicroserviceConfigurationException ex =
            Assert.Throws<MicroserviceConfigurationException>(() =>
            {
                using (var host = new MicroserviceHost<SomeMicroserviceMock>(serviceMock, options))
                {
                    host.Open();
                }
            });
        Assert.Equal("The Eventbus (RabbitMQ service) cannot be reached.", ex.Message);
    }
}