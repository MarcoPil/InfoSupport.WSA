using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoSupport.WSA.Infrastructure.Test.dummies
{
    public class EventDispatcherMock : EventDispatcher
    {
        public bool TestEventHandlerHasBeenCalled = false;
        public bool AnotherEventHandlerHasBeenCalled = false;
        public long SomeTimestamp = 0;
        public int SomeValue = 0;

        public void TestEventHandler(TestEvent te)
        {
            TestEventHandlerHasBeenCalled = true;
            SomeTimestamp = te.Timestamp;
        }

        public void AnotherEventHandler(AnotherEvent te)
        {
            AnotherEventHandlerHasBeenCalled = true;
            SomeTimestamp = te.Timestamp;
            SomeValue = te.SomeValue;
        }

        public void NoEventHandler(string noEvent)
        {

        }

        public void NoEventHandler(object noEvent)
        {

        }
    }
}
