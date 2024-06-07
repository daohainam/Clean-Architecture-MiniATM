using MiniATM.Entities.Exceptions;

namespace MiniATM.Entities
{
    public class BankAccount
    {
        private double balance;

        public required string Id { get; set; }
        public required Guid CustomerId { get; set; }
        public double Balance { 
            get 
            { 
                return balance; 
            } 
            set 
            { 
                if (value < MinimumRequiredAmount) throw new InvalidBalanceException();
                balance = value;
            } 
        }
        public required string Currency { get; set; }
        public bool IsLocked { get; set; }
        public double MinimumRequiredAmount { get; set; } // can be negative
    }
}
