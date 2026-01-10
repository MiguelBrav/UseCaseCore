using System;
using System.Collections.Generic;
using System.Text;

namespace UseCaseCore.DomainEvents
{
    public abstract class DomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
