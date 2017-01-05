using System;

namespace InfoSupport.WSA.Infrastructure
{
    public class MicroserviceException : Exception
    {
        public MicroserviceException() { }
        public MicroserviceException(string message) : base(message) { }
        public MicroserviceException(string message, Exception innerException) : base(message, innerException) { }
    }
}