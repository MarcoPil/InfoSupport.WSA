using InfoSupport.WSA.Common;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InfoSupport.WSA.Infrastructure
{
    public class EventDispatcher : EventBusBase
    {
        public DispatcherModel DispatcherModel { get; }

        public EventDispatcher(BusOptions options = default(BusOptions)) : base(options)
        {
            DispatcherModel = new DispatcherModel();
            PopulateDispatcherModel();
        }

        private void PopulateDispatcherModel()
        {
            var customType = this.GetType();

            // Populate Routing Keys
            var routingKeyAttrs = customType.GetTypeInfo().GetCustomAttributes<RoutingKeyAttribute>();
            foreach (var attr in routingKeyAttrs)
            {
                DispatcherModel.AddRoutingKey(attr.RoutingKey);
            }

            // Populate Event Handlers
            var customMethods = customType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            foreach (var method in customMethods)
            {
                if (method.GetParameters().Length == 1)
                {
                    var paramType = method.GetParameters().First().ParameterType;
                    if (typeof(DomainEvent).IsAssignableFrom(paramType))
                    {
                        DispatcherModel.AddHandler(
                            paramType.FullName,
                            new DispatchHandler(this, method, paramType));
                    }
                    if (paramType == typeof(Newtonsoft.Json.Linq.JObject))
                    {
                        DispatcherModel.AddHandler(
                            "default",
                            new DispatchHandler(this, method, typeof(object)));
                    }
                }
            }
        }

        public override void Open()
        {
            // Open a RabbitMQ connection
            base.Open();

            // Start listening for incomning commands
            string queueName = BusOptions.QueueName;
            if (queueName == null)
            {
                queueName = Channel.QueueDeclare().QueueName;
            }
            else
            {
                Channel.QueueDeclare(queue: queueName);
            }

            if (DispatcherModel.RoutingKeys.Count() == 0)
            {
                Channel.QueueBind(exchange: BusOptions.ExchangeName,
                                  queue: queueName,
                                  routingKey: "#");
            }
            else
            {
                foreach (var routingKey in DispatcherModel.RoutingKeys)
                {
                    Channel.QueueBind(exchange: BusOptions.ExchangeName,
                                      queue: queueName,
                                      routingKey: routingKey);
                }
            }

            var consumer = new EventingBasicConsumer(Channel);
            consumer.Received += EventReceived;

            Channel.BasicConsume(queue: queueName,
                                 noAck: true,
                                 consumer: consumer);
        }

        protected virtual void EventReceived(object sender, BasicDeliverEventArgs e)
        {
            var eventType = e.BasicProperties.Type;
            var message = Encoding.UTF8.GetString(e.Body);
            DispatcherModel.DispatchEvent(eventType, message);
        }
    }
}
