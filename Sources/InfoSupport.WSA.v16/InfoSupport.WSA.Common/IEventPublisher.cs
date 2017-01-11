using InfoSupport.WSA.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoSupport.WSA.Infrastructure
{
    /// <summary>
    /// Implementations for this interface should publish Domain Events on the Event Bus.
    /// </summary>
    public interface IEventPublisher : IDisposable
    {
        void Publish(DomainEvent domainEvent);
    }
}
