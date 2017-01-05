using InfoSupport.WSA.Common;
using InfoSupport.WSA.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;

namespace InfoSupport.WSA.Logging.Test.IntegrationTests.Dummies
{
    public class ReplayEventReceiverMock : EventDispatcher
    {
        public List<DomainEvent> ReceivedEvents = new List<DomainEvent>();
        public List<Newtonsoft.Json.Linq.JObject> WronglyReceivedEvents = new List<Newtonsoft.Json.Linq.JObject>();

        public ReplayEventReceiverMock(BusOptions options = default(BusOptions)) : base(options) { }


        public void Number1EventHandler(Number1Event evt)
        {
            ReceivedEvents.Add(evt);
        }
        public void Number2EventHandler(Number2Event evt)
        {
            ReceivedEvents.Add(evt);
        }
        public void Name1EventHandler(Name1Event evt)
        {
            ReceivedEvents.Add(evt);
        }
        public void Name2EventHandler(Name2Event evt)
        {
            ReceivedEvents.Add(evt);
        }

        public void DefaultHandler(Newtonsoft.Json.Linq.JObject obj)
        {
            WronglyReceivedEvents.Add(obj);
        }

        protected override void EventReceived(object sender, BasicDeliverEventArgs e)
        {
            base.EventReceived(sender, e);
        }
    }
}
