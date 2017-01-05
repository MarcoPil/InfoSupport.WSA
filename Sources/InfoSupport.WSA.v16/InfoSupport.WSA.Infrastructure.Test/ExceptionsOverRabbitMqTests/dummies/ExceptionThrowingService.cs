using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoSupport.WSA.Infrastructure.Test.ExceptionsOverRabbitMqTests.dummies
{
    public class ExceptionThrowingService : IExceptionThrowingService
    {
        public void DoSomeWork(WorkCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
