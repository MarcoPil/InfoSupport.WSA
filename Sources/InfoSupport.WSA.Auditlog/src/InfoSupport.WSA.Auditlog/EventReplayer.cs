using InfoSupport.WSA.Infrastructure;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoSupport.WSA.Logging.Model;

namespace InfoSupport.WSA.Logging
{
    public class EventReplayer : EventBusBase, IEventReplayer
    {
        /// <summary>
        /// Each EventReplayer creates its own connection to rabbitMQ.
        /// </summary>
        /// <param name="options">the configuration of the RabbitMQ connection. If none are passed, the default BusOptions are being used.</param>
        public EventReplayer(BusOptions options = default(BusOptions)) : base(options)
        {
        }

        public string ExchangeName
        {
            get { return BusOptions.ExchangeName;  }
            set { BusOptions.ExchangeName = value; }
        }

        public void Start()
        {
            Open();
        }

        public void ReplayLogEntry(LogEntry logEntry)
        {
            // set metadata
            var props = Channel.CreateBasicProperties();
            props.Timestamp = new AmqpTimestamp(logEntry.Timestamp);
            props.Type = logEntry.EventType;
            // set payload
            var buffer = Encoding.UTF8.GetBytes(logEntry.EventJson);
            // publish event
            Channel.BasicPublish(exchange: BusOptions.ExchangeName,
                                     routingKey: logEntry.RoutingKey,
                                     basicProperties: props,
                                     body: buffer);
        }


        public void Stop()
        {
            Dispose();
        }
    }
}
