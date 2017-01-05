using InfoSupport.WSA.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoSupport.WSA.Infrastructure.Test.dummies
{
    public class AnotherEvent : DomainEvent
    {
        public AnotherEvent() : base("Dummy.AnotherEvent") { }

        public int SomeValue { get; set; }
    }
}
