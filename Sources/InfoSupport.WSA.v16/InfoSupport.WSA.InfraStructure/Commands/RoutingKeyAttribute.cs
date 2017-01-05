using System;

namespace InfoSupport.WSA.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RoutingKeyAttribute : Attribute
    {
        public string RoutingKey { get; }

        public RoutingKeyAttribute(string routingKey)
        {
            RoutingKey = routingKey;
        }
    }
}