using MiniATM.UseCase.Repositories;
using MiniATM.UseCase.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseCaseTests
{
    public class SimpleTransactionUnitOfWork(ITransactionRepository transactionRepository, IBankAccountRepository bankAccountRepository) : ITransactionUnitOfWork
    {
        public ITransactionRepository TransactionRepository => transactionRepository;

        public IBankAccountRepository BankAccountRepository => bankAccountRepository;

        public Task BeginTransactionAsync()
        {
            return Task.CompletedTask;
        }

        public Task CancelAsync()
        {
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync()
        {
            return Task.CompletedTask;
        }
    }
}
