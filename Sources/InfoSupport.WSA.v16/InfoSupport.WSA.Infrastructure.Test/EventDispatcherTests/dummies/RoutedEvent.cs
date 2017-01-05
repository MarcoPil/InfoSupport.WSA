using InfoSupport.WSA.Common;

namespace InfoSupport.WSA.Infrastructure.Test.dummies
{
    public class RoutedEvent : DomainEvent
    {
        public RoutedEvent() : base("WSA.Routed.RoutedEvent") { }
    }
}