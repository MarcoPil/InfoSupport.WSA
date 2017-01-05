using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoSupport.WSA.Infrastructure.Test.dummies
{
    public class DefaultingDispatcherMock : EventDispatcher
    {
        public DefaultingDispatcherMock(BusOptions options = null) : base(options) { }

        public bool DefaultHandlerHasBeenCalled = false;
        public Newtonsoft.Json.Linq.JObject DefaultHandlerObject = null;

        public void DefaultHandler(Newtonsoft.Json.Linq.JObject obj)
        {
            DefaultHandlerHasBeenCalled = true;
            DefaultHandlerObject = obj;
        }
    }
}
