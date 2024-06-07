using MiniATM.UseCase.Repositories;
using MiniATM.UseCase.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniATM.Infrastructure.InMemory
{
    public class InMemoryTransactionUnitOfWork : ITransactionUnitOfWork
    {
        public ITransactionRepository TransactionRepository { get; } = new InMemoryTransactionRepository();

        public IBankAccountRepository BankAccountRepository { get; } = new InMemoryBankAccountRepository();

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
