using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoSupport.WSA.Infrastructure.Test.dummies
{
    [RoutingKey("WSA.Routed.*")]
    [RoutingKey("Multiple.#")]
    public class RoutedDispatcherMock : EventDispatcher
    {
        public RoutedDispatcherMock(BusOptions options = null) : base(options) { }

        public bool TestEventHandlerHasBeenCalled = false;
        public bool RoutedEventHandlerHasBeenCalled = false;

        public void TestEventHandler(TestEvent te)
        {
            TestEventHandlerHasBeenCalled = true;
        }

        public void RoutedEventHandler(RoutedEvent te)
        {
            RoutedEventHandlerHasBeenCalled = true;
        }

    }
}
