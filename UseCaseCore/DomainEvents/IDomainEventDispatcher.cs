using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UseCaseCore.DomainEvents
{
    public interface IDomainEventDispatcher
    {
        Task Publish(DomainEvent domainEvent);
    }
}
