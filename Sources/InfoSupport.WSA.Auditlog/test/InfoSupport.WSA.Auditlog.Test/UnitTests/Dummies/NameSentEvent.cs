using InfoSupport.WSA.Common;

namespace InfoSupport.WSA.Logging.Test.Dummies
{
    class NameSentEvent : DomainEvent
    {
        public NameSentEvent() : base("Eventlogging.Test.NameSentEvent")
        {
        }

        public string Name { get; set; }
    }
}