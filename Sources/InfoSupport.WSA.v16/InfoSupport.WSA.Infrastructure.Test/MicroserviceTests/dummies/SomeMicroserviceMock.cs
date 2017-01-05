using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InfoSupport.WSA.Infrastructure.Test.dummies
{
    public class SomeMicroserviceMock : ISomeMicroserviceMock
    {
        public bool TestCommandHandlerHasBeenCalled = false;
        public TestCommand TestCommandHandlerReceivedSomeCommand = null;
        public bool SomeCommandHandlerHasBeenCalled = false;
        public SomeCommand SomeCommandHandlerReceivedSomeCommand = null;
        public bool NotAHandlerHasBeenCalled = false;
        public string NotAHandlerReceivedSomeCommand = null;
        public bool AlsoNotAHandlerHasBeenCalled = false;
        public SomeCommand AlsoNotAHandlerReceivedSomeCommand = null;
        public EventWaitHandle ReceivedFlag = new AutoResetEvent(false);

        public void TestCommandHandler(TestCommand command)
        {
            TestCommandHandlerHasBeenCalled = true;
            TestCommandHandlerReceivedSomeCommand = command;
            ReceivedFlag.Set();
        }

        public void SomeCommandHandler(SomeCommand command)
        {
            SomeCommandHandlerHasBeenCalled = true;
            SomeCommandHandlerReceivedSomeCommand = command;
            ReceivedFlag.Set();
        }

        public void NotAHandler(string arg) // no Command argument
        {
            NotAHandlerHasBeenCalled = true;
            NotAHandlerReceivedSomeCommand = arg;
            ReceivedFlag.Set();
        }

        public void AlsoNotAHandler(SomeCommand command)    // not listed in the microservice Interface
        {
            AlsoNotAHandlerHasBeenCalled = true;
            AlsoNotAHandlerReceivedSomeCommand = command;
            ReceivedFlag.Set();
        }
    }
}
