using System;

namespace InfoSupport.WSA.Infrastructure
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class MicroserviceAttribute : Attribute
    {
        public string QueueName { get; }

        public MicroserviceAttribute(string queueName = null)
        {
            QueueName = queueName;
        }
    }
}