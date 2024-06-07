using MiniATM.Entities;
using MiniATM.UseCase.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace MiniATM.Infrastructure.InMemory
{
    public class InMemoryBankAccountRepository : IBankAccountRepository
    {
        private List<BankAccount> bankAccounts;

        public InMemoryBankAccountRepository()
        {
            bankAccounts =
            [
                new BankAccount() { 
                    Id = "001",
                    Balance = 10000,
                    Currency = "VND",
                    CustomerId = Guid.Empty,
                    IsLocked = false,
                    MinimumRequiredAmount = 0
                },
                new BankAccount()
                {
                    Id = "002",
                    Balance = 10000,
                    Currency = "VND",
                    CustomerId = Guid.Empty,
                    IsLocked = false,
                    MinimumRequiredAmount = 0
                },
            ];
        }

        public InMemoryBankAccountRepository(List<BankAccount> initBankAccounts)
        {
            bankAccounts = initBankAccounts;
        }

        public Task<IEnumerable<BankAccount>> FindByCustomerIdAsync(Guid customerId)
        {
            return Task.FromResult(bankAccounts.Where(ba => ba.CustomerId == customerId));
        }

        public Task<BankAccount?> FindByIdAsync(string accountId)
        {
            return Task.FromResult(bankAccounts.Where(ba => ba.Id == accountId).FirstOrDefault());
        }

        public Task UpdateAsync(BankAccount fromAccount)
        {
            var bankAccount = bankAccounts.Where(ba => ba.Id == fromAccount.Id).FirstOrDefault();
            if (bankAccount != null) { 
                bankAccounts.Remove(bankAccount);
                bankAccounts.Add(fromAccount);
            }

            return Task.CompletedTask;
        }
    }
}
