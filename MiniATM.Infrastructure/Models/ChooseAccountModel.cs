using MiniATM.Entities;

namespace MiniATM.Infrastructure.Models
{
    public class ChooseAccountModel
    {
        public required IEnumerable<BankAccount> BankAccounts { get; set; }
        public required string ReturnUrl { get; set; }

        public bool IsEmpty => !BankAccounts.Any();
    }
}
