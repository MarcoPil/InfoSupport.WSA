using System;
using InfoSupport.WSA.Infrastructure;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;
using InfoSupport.WSA.Logging.Model;

namespace InfoSupport.WSA.Logging
{
    public class AuditlogEventListener : EventBusBase
    {
        private readonly ILogRepository _logRepo;

        public AuditlogEventListener(ILogRepository logRepo, BusOptions options = default(BusOptions)) : base(options)
        {
            _logRepo = logRepo;
        }

        public void Start()
        {
            Open();    // Opens a RabbitMQ connection

            var queueName = Channel.QueueDeclare().QueueName;
            Channel.QueueBind(exchange: BusOptions.ExchangeName,
                              queue: queueName,
                              routingKey: "#");

            var consumer = new EventingBasicConsumer(Channel);
            consumer.Received += EventReceived;

            Channel.BasicConsume(queue: queueName,
                                 noAck: true,
                                 consumer: consumer);
        }

        private void EventReceived(object sender, BasicDeliverEventArgs e)
        {
            Console.WriteLine("[{0}] - Event Received", DateTime.Now);
            //Console.WriteLine(JsonConvert.SerializeObject(e));
            Console.WriteLine("-----");
            try
            {
                string eventType = e.BasicProperties.Type;
                long timestamp = e.BasicProperties.Timestamp.UnixTime;
                string eventJson = Encoding.UTF8.GetString(e.Body);
                Console.WriteLine(eventJson);
                dynamic obj = JsonConvert.DeserializeObject(eventJson);
                string routingKey = obj.RoutingKey;

                LogEntry logEntry = new LogEntry
                {
                    EventType = eventType,
                    Timestamp = timestamp,
                    RoutingKey = routingKey,
                    EventJson = eventJson,
                };

                _logRepo.AddEntry(logEntry);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                // If something goes wrong, we fail silently
            }
            Console.WriteLine("---End Event--");
        }
    }
}