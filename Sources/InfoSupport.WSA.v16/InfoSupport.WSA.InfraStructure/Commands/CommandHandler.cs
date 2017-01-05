using Newtonsoft.Json;
using System;
using System.Reflection;

namespace InfoSupport.WSA.Infrastructure
{
    internal class CommandHandler<T>
    {
        private MethodInfo _method;
        private Type _commandType;

        public CommandHandler(MethodInfo method, Type commandType)
        {
            _method = method;
            _commandType = commandType;
        }

        internal ServiceResponse DispatchCall(T instance, string jsonMessage)
        {
            var command = JsonConvert.DeserializeObject(jsonMessage, _commandType);

            try
            {
                var response = _method.Invoke(instance, new object[] { command });

                return new ServiceResponse(response, ResponseType.OK); 
            }
            catch (TargetInvocationException ex)
            {
                return new ServiceResponse(ex.InnerException, ResponseType.FunctionalException);
            }
        }
    }
}