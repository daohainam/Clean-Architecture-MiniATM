using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniATM.UseCase
{
    public interface ITransactionManager
    {
        Task<TransactionResult> WithdrawCashAsync(string accountId, double amount);
        Task<TransactionResult> TransferAsync(string fromAccountId, string toAccountId, double amount);
    }
}
