using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InfoSupport.WSA.Infrastructure.Test.dummies
{
    public class CallbackMock : ICallbackMock
    {
        public TestResponse SomeMethod(RequestCommand request)
        {
            return new TestResponse { Greeting = $"Hello, {request.Name}" };
        }

        public TestResponse SlowMethod(SlowRequestCommand request)
        {
            Thread.Sleep(1000);
            return new TestResponse { Greeting = $"Hello, {request.Name}" };
        }
    }
}
