using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniATM.Entities
{
    public class Transaction
    {
        public required Guid Id { get; set; }
        public required TransactionTypes TransactionTypes { get; set; }
        public required string AccountId { get; set; } 
        public required double Amount { get; set; }
        public DateTime DateTimeUTC { get; set; }
        public required string Notes { get; set; }
    }
}
