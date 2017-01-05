using System;
using System.Reflection;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;
using InfoSupport.WSA.Common;

namespace InfoSupport.WSA.Infrastructure
{
    internal class DispatchHandler
    {
        private object _instance;
        private MethodInfo _method;
        private Type _paramType;

        public DispatchHandler(object instance, MethodInfo method, Type paramType)
        {
            _instance = instance;
            _method = method;
            _paramType = paramType;
        }

        internal void DispatchEvent(string jsonMessage)
        {
            var domainEvent = JsonConvert.DeserializeObject(jsonMessage, _paramType);

            _method.Invoke(_instance, new object[] { domainEvent });
        }
    }
}