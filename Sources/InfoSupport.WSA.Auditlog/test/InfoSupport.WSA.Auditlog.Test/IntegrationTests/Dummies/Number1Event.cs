using InfoSupport.WSA.Common;

namespace InfoSupport.WSA.Logging.Test.IntegrationTests.Dummies
{
    public class Number1Event : DomainEvent
    {
        public Number1Event() : base("IntegrationTest.Pub1.Number1Event")
        {
        }

        public int Number { get; set; }
    }
}