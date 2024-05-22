using System.ComponentModel.DataAnnotations;

namespace MiniATM.Infrastructure.SqlServer.Repositories.SqlServer.DataContext
{
    public class BankAccount
    {
        [MaxLength(50)]
        public required string Id { get; set; }
        public required Guid CustomerId { get; set; }
        public double Balance { get; set; } = 0;
        [MaxLength(3)]
        public required string Currency { get; set; }
        public bool IsLocked { get; set; }
        public double MinimumRequiredAmount { get; set; }

        public ICollection<Transaction> Transactions { get; } = [];
    }
}
