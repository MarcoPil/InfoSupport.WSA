using InfoSupport.WSA.Common;

namespace InfoSupport.WSA.Logging.Test.IntegrationTests.Dummies
{
    public class Name1Event : DomainEvent
    {
        public Name1Event() : base("IntegrationTest.Pub1.Name1Event")
        {
        }

        public string Name { get; set; }
    }
}