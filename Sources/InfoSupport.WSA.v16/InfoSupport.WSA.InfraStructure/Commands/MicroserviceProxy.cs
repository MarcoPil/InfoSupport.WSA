using System;
using InfoSupport.WSA.Infrastructure;
using Newtonsoft.Json;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading;

public class MicroserviceProxy : EventBusBase
{
    public string EndpointName { get; }

    public MicroserviceProxy(string endpointName, BusOptions options = default(BusOptions)) : base(options)
    {
        EndpointName = endpointName;

        try
        {
            Open();
        }
        catch
        {
            Dispose();
            throw;
        }
    }

    public void Execute(object command)
    {
        Execute<object>(command);
    }

    public T Execute<T>(object command)
    {
        Channel.QueueDeclare(queue: EndpointName,
                             durable: false, exclusive: false, autoDelete: false, arguments: null);
        var callbackQueueName = Channel.QueueDeclare().QueueName;
        EventWaitHandle responseReceived = new AutoResetEvent(false);
        BasicDeliverEventArgs callback = null;

        // set metadata
        var props = Channel.CreateBasicProperties();
        props.Type = command.GetType().FullName;
        props.ReplyTo = callbackQueueName;

        // set payload
        string message = JsonConvert.SerializeObject(command);
        var buffer = Encoding.UTF8.GetBytes(message);

        // Start Listening for callback
        var consumer = new EventingBasicConsumer(Channel);
        consumer.Received += (object sender, BasicDeliverEventArgs e) =>
        {
            callback = e;
            responseReceived.Set();
        };
        Channel.BasicConsume(queue: callbackQueueName,
                             noAck: true,
                             consumer: consumer);

        // Send Command
        Channel.BasicPublish(exchange: "",
                             routingKey: EndpointName,
                             basicProperties: props,
                             body: buffer);

        // Wait for result
        responseReceived.WaitOne();

        var responseType = callback.BasicProperties.ContentType;
        var eventType = callback.BasicProperties.Type;
        string body = Encoding.UTF8.GetString(callback.Body);
        switch (responseType)
        {
            case "OK":
                if (eventType == "void")
                {
                    return default(T);
                }
                else
                {
                    var result = JsonConvert.DeserializeObject<T>(body);
                    return result; 
                }
            case "FunctionalException":
                var ex = (Exception) JsonConvert.DeserializeObject(body, Type.GetType(eventType));
                throw ex;
            case "InternalError":
                throw new MicroserviceException(body);
            default:
                throw new MicroserviceException($"Unknown response type: {responseType}");
        }
    }
}