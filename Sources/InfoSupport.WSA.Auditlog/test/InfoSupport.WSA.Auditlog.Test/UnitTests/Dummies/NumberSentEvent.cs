using InfoSupport.WSA.Common;

namespace InfoSupport.WSA.Logging.Test.Dummies
{
    class NumberSentEvent : DomainEvent
    {
        public NumberSentEvent() : base("Eventlogging.Test.NumberSentEvent")
        {
        }

        public int Number { get; set; }
    }
}