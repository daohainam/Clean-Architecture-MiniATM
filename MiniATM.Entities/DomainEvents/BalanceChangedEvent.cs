using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniATM.Entities.DomainEvents
{
    public class BalanceChangedEvent: DomainEvent
    {
        public required string AccountId { get; set; }
        public required double NewBalance { get; set; }
        public required double OldBalance { get; set; }
    }
}
