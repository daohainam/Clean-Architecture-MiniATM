using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniATM.Entities.DomainEvents
{
    public class DomainEvent
    {
        public required DateOnly EventTimeUtc { get; set; }
    }
}
