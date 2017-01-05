using System;

namespace InfoSupport.WSA.Infrastructure
{
    internal class ServiceResponse
    {
        public ResponseType ResponseType { get; set; }
        public object Value { get; set; }

        public ServiceResponse(object response, ResponseType type)
        {
            ResponseType = type;
            Value = response;
        }

        public string TypeName
        {
            get { return Value?.GetType().FullName ?? "void"; }
        }
    }
    internal enum ResponseType
    {
        OK,
        FunctionalException,
        InternalError,
    }
}