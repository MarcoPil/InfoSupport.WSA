using InfoSupport.WSA.Common;

namespace InfoSupport.WSA.Infrastructure.Test
{
    public class TestEvent : DomainEvent
    {
        public TestEvent() : base("TestEvent")
        {

        }
    }
}