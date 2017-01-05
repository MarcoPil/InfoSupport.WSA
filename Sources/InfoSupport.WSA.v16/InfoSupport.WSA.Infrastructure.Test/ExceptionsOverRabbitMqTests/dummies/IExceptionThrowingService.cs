using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoSupport.WSA.Infrastructure.Test.ExceptionsOverRabbitMqTests.dummies
{
    [Microservice]
    interface IExceptionThrowingService
    {
        void DoSomeWork(WorkCommand command);
    }
}
