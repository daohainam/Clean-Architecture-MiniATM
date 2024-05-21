namespace MiniATM.Entities
{
    public class BankAccount
    {
        public required string Id { get; set; }
        public required Guid CustomerId { get; set; }
        public double Balance { get; set; } = 0;
        public required string Currency { get; set; }
        public bool IsLocked { get; set; }
        public double MinimumRequiredAmount { get; set; } // can be negative
    }
}
