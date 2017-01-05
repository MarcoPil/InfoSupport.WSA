using InfoSupport.WSA.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;


public class BusOptionsTest
{
    [Fact]
    public void DefaultBusoptions()
    {
        // Act
        var result = new BusOptions();

        // Assert
        Assert.Equal("WSA.DefaultEventBus", result.ExchangeName);
        Assert.Equal(null, result.QueueName);
        Assert.Equal("localhost", result.HostName);
        Assert.Equal(5672, result.Port);
        Assert.Equal("guest", result.UserName);
        Assert.Equal("guest", result.Password);
    }

    [Fact]
    public void BusOptionsToString()
    {
        // Arrange
        var target = new BusOptions
        {
            ExchangeName = "Eventbus",
            QueueName = "ListenQueue",
            HostName = "ServerName",
            Port = 12345,
            UserName = "Jan",
            Password = "&Alleman",
        };

        // Act
        var result = target.ToString();

        //
        // Assert
        Assert.Equal("{\r\n  \"ExchangeName\": \"Eventbus\",\r\n  \"QueueName\": \"ListenQueue\",\r\n  \"HostName\": \"ServerName\",\r\n  \"Port\": 12345,\r\n  \"UserName\": \"Jan\",\r\n  \"Password\": \"&Alleman\"\r\n}", result.ToString());
    }
}