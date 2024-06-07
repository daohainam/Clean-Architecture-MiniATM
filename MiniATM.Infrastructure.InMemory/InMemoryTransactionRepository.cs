using MiniATM.Entities;
using MiniATM.UseCase.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniATM.Infrastructure.InMemory
{
    public class InMemoryTransactionRepository : ITransactionRepository
    {
        private readonly List<Transaction> transactions;

        public InMemoryTransactionRepository() {
            transactions = [];
        }


        public Task Add(Transaction transaction)
        {
            transactions.Add(transaction);

            return Task.CompletedTask;
        }
    }
}
