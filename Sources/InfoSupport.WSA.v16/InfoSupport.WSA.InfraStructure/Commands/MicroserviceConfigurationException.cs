using System;

namespace InfoSupport.WSA.Infrastructure
{
    public class MicroserviceConfigurationException : Exception
    {
        public MicroserviceConfigurationException() { }
        public MicroserviceConfigurationException(string message) : base(message) { }
        public MicroserviceConfigurationException(string message, Exception inner) : base(message, inner) { }
    }
}