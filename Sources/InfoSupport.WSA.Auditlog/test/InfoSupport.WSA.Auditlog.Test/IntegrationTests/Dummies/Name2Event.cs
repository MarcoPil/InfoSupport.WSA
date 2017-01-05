using InfoSupport.WSA.Common;

namespace InfoSupport.WSA.Logging.Test.IntegrationTests.Dummies
{
    public class Name2Event : DomainEvent
    {
        public Name2Event() : base("IntegrationTest.Pub2.Name2Event")
        {
        }

        public string Name { get; set; }
    }
}