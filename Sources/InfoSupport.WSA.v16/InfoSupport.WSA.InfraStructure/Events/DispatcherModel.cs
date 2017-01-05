using System;
using System.Collections.Generic;
using System.Linq;

namespace InfoSupport.WSA.Infrastructure
{
    public class DispatcherModel
    {
        private Dictionary<string, DispatchHandler> _handlers;
        private List<string> _routingKeys;
        public IEnumerable<string> Handlers => _handlers.Keys;
        public IEnumerable<string> RoutingKeys => _routingKeys;

        public DispatcherModel()
        {
            _handlers = new Dictionary<string, DispatchHandler>();
            _routingKeys = new List<string>();
        }

        internal void AddHandler(string eventName, DispatchHandler dispatchHandler)
        {
            _handlers.Add(eventName, dispatchHandler);
        }

        internal void DispatchEvent(string eventName, string jsonMessage)
        {
            if (_handlers.ContainsKey(eventName))
            {
                _handlers[eventName].DispatchEvent(jsonMessage);
            }
            else if (_handlers.ContainsKey("default"))
            {
                if (jsonMessage.Last() == '}')
                {
                    jsonMessage = jsonMessage.Substring(0, jsonMessage.Length - 1) + $",\"TypeName\":\"{eventName}\"}}";
                }
                _handlers["default"].DispatchEvent(jsonMessage);
            }
        }

        internal void AddRoutingKey(string routingKey)
        {
            _routingKeys.Add(routingKey);
        }
    }
}