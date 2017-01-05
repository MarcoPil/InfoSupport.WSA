using InfoSupport.WSA.Common;

namespace InfoSupport.WSA.Logging.Test.IntegrationTests.Dummies
{
    public class Number2Event : DomainEvent
    {
        public Number2Event() : base("IntegrationTest.Pub2.Number2Event")
        {
        }

        public int Number { get; set; }
    }
}