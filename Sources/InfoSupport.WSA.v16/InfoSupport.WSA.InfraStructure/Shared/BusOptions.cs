using Newtonsoft.Json;
using System;

namespace InfoSupport.WSA.Infrastructure
{
    /// <summary>
    /// The BusOptions are used for configuring a connection to RabbitMQ.
    /// </summary>
    public class BusOptions
    {
        /// <summary>
        /// Default: "WSA.DefaultEventBus"
        /// </summary>
        public string ExchangeName { get; set; }
        /// <summary>
        /// The QueueName is only relevant for listeners. A null-value means that the listener will generate a random new queuename
        /// Default QueueName: null 
        /// </summary>
        public string QueueName { get; set; }
        /// <summary>
        /// Default HostName: "localhost"
        /// </summary>
        public string HostName { get; set; }
        /// <summary>
        /// Default Port: 5672
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Default UserName: "guest"
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Default Password: "guest"
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Initializes with default BusOptions
        /// </summary>
        public BusOptions()
        {
            ExchangeName = "WSA.DefaultEventBus";
            QueueName = null;
            HostName = "localhost";
            Port = 5672;
            UserName = "guest";
            Password = "guest";
        }

        /// <summary>
        /// Read BusOptions from environment variables:
        /// eventbus-exchangename, eventbus-queuename, eventbus-hostname, eventbus-port, eventbus-username, eventbus-password
        /// </summary>
        /// <returns></returns>
        public static BusOptions CreateFromEnvironment()
        {
            var busOptions = new BusOptions();

            busOptions.ExchangeName = Environment.GetEnvironmentVariable("eventbus-exchangename") ?? busOptions.ExchangeName;
            busOptions.QueueName = Environment.GetEnvironmentVariable("eventbus-queuename") ?? busOptions.QueueName;
            busOptions.HostName = Environment.GetEnvironmentVariable("eventbus-hostname") ?? busOptions.HostName;
            int portnumber = 0;
            if (int.TryParse(Environment.GetEnvironmentVariable("eventbus-port"), out portnumber))
            {
                busOptions.Port = portnumber;
            }
            busOptions.UserName = Environment.GetEnvironmentVariable("eventbus-username") ?? busOptions.UserName;
            busOptions.Password = Environment.GetEnvironmentVariable("eventbus-password") ?? busOptions.Password;

            return busOptions;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}