using MiniATM.UseCase.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniATM.UseCase.UnitOfWork
{
    public interface ITransactionUnitOfWork
    {
        ITransactionRepository TransactionRepository { get; }
        IBankAccountRepository BankAccountRepository { get; }

        Task BeginTransactionAsync();
        Task SaveChangesAsync();
        Task CancelAsync(); // this method should be called ASAP before leaving, a ITransactionUnitOfWork implementation should implement IDisposable to handle that
    }
}
