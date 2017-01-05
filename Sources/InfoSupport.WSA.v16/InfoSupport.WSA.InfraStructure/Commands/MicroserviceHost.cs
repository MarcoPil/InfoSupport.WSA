using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace InfoSupport.WSA.Infrastructure
{
    /// <summary>
    /// Provides a host for microservices.
    /// Communication is done through RabbitMQ RPC calls. This Host listens for commands on all queues that are configured in the [Microservce(queueName)] atttributes and the queue name that is configured in the BusOptions
    /// </summary>
    /// <typeparam name="T">The type of the implementation of the Microserice</typeparam>
    public class MicroserviceHost<T> : EventBusBase
        where T: class, new()
    {
        private T _instance;

        public ServiceModel<T> ServiceModel { get; private set; }

        public MicroserviceHost() : this(null, null) { }
        public MicroserviceHost(T singletonInstance) : this(singletonInstance, null) { }
        public MicroserviceHost(BusOptions busOptions) : this(null, busOptions) { }
        public MicroserviceHost(T singletonInstance, BusOptions busOptions) : base(busOptions)
        {
            _instance = singletonInstance;
            ServiceModel = new ServiceModel<T>();
            AddQueueNamesToServiceModel();
            AddCommandHandlersToServiceModel();
        }

        private void AddQueueNamesToServiceModel()
        {
            // gather queue names from [MicroService(queueName)]-attributes
            foreach (var Interface in AllInterfacesOf<T>())
            {
                var attrs = Interface.GetTypeInfo().GetCustomAttributes<MicroserviceAttribute>();
                if (attrs.Any())
                {
                    ServiceModel.AddIfNotEmpty(attrs.First().QueueName);
                }
            }
            // gather queue name from BusOptions
            ServiceModel.AddIfNotEmpty(BusOptions.QueueName);
        }

        private void AddCommandHandlersToServiceModel()
        {
            var microserviceInterfaces = AllInterfacesOf<T>().Where(HasMicroserviceAttribute);

            if (!microserviceInterfaces.Any())
            {
                throw new MicroserviceConfigurationException("No [MicroService] interfaces have been found.");
            }
            else
            {
                var commandHandlerList =
                    from Interface in microserviceInterfaces
                    from interfaceMethod in Interface.GetMethods()
                    where interfaceMethod.GetParameters().Length == 1
                    let commandType = GetCommandType(interfaceMethod)
                    select new KeyValuePair<string, CommandHandler<T>>
                (
                    commandType.FullName,
                    new CommandHandler<T>(interfaceMethod, commandType)
                );

                if (!commandHandlerList.Any())
                {
                    throw new MicroserviceConfigurationException("No Handlers can be found in the Microservice interface.");
                }
                else
                {
                    ServiceModel.Add(commandHandlerList);
                }
            }

        }
        #region PopulateServiceModel() - helper methods
        private static Type[] AllInterfacesOf<U>()
        {
            return typeof(U).GetInterfaces();
        }
        private static bool HasMicroserviceAttribute(Type interfaceType)
        {
            return interfaceType.GetTypeInfo().GetCustomAttributes<MicroserviceAttribute>().Any();
        }
        private static Type GetCommandType(MethodInfo method)
        {
            return method.GetParameters().First().ParameterType;
        }
        #endregion PopulateServiceModel() - helper methods

        public override void Open()
        {
            if (!ServiceModel.QueueNames.Any())
            {
                throw new MicroserviceConfigurationException("No queue name is configured in the MicroserviceAtrribute on any Microservice interface nor in the Busoptions.");
            }

            // Open a RabbitMQ connection
            base.Open();
            
            // Start listening for incomning commands
            foreach (var queueName in ServiceModel.QueueNames)
            {
                Channel.QueueDeclare(queue:queueName, 
                                     durable:false, exclusive:false, autoDelete:false, arguments:null);

                var consumer = new EventingBasicConsumer(Channel);
                consumer.Received += EventReceived;

                Channel.BasicConsume(queue: queueName,
                                     noAck: true,
                                     consumer: consumer);
            }
        }

        private void EventReceived(object sender, BasicDeliverEventArgs e)
        {
            var instance = _instance ?? new T();    // InstanceContext=Singleton ?? SingleCall
            var eventType = e.BasicProperties.Type;
            var replyTo = e.BasicProperties.ReplyTo;
            var message = Encoding.UTF8.GetString(e.Body);

            ServiceResponse responseMessage = ServiceModel.DispatchCall(instance, eventType, message);

            SendResponse(replyTo, responseMessage);
        }

        private void SendResponse(string replyTo, ServiceResponse serviceResponse)
        {
            if (replyTo != null)
            {
                // set metadata
                var props = Channel.CreateBasicProperties();
                props.ContentType = serviceResponse.ResponseType.ToString();
                props.Type = serviceResponse.TypeName;
                //props.ReplyTo = "HEEE";
                
                // set payload
                var responseMessage = JsonConvert.SerializeObject(serviceResponse.Value);
                var responseBuffer = Encoding.UTF8.GetBytes(responseMessage);
                // publish event
                Channel.BasicPublish(exchange: "",
                                     routingKey: replyTo,
                                     basicProperties: props,
                                     body: responseBuffer);
            }
        }
    }
}