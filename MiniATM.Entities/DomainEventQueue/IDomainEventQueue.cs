using MiniATM.Entities.DomainEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniATM.Entities.DomainEventQueue
{
    public interface IDomainEventQueue<T> where T: DomainEvent
    {
        void Enqueue(T evt);
    }
}
