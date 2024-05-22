using MiniATM.Entities;
using MiniATM.UseCase.Repositories;
using MiniATM.UseCase.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniATM.UseCase
{
    public class CashWithdrawalManager(
        ITransactionUnitOfWork transactionUnitOfWork,
        ICashStorage cashStorage,
        bool useSafeCashWithdrawal = true
            ) : ICashWithdrawalManager
    {
        private readonly ITransactionUnitOfWork transactionUnitOfWork = transactionUnitOfWork ?? throw new ArgumentNullException(nameof(transactionUnitOfWork));
        private readonly ICashStorage cashStorage = cashStorage ?? throw new ArgumentNullException(nameof(cashStorage));

        public async Task<TransactionResult> WithdrawAsync(string accountId, double amount)
        {
            try
            {
                await transactionUnitOfWork.BeginTransactionAsync();

                var fromAccount = await transactionUnitOfWork.BankAccountRepository.FindByIdAsync(accountId);
                if (fromAccount == null || fromAccount.IsLocked)
                {
                    return TransactionResult.SourceNotFound;
                }

                var balanceLeft = fromAccount.Balance - amount;
                if (balanceLeft < fromAccount.MinimumRequiredAmount)
                {
                    return TransactionResult.BalanceTooLow;
                }

                if (!cashStorage.IsCashAmountAvailable(amount))
                {
                    return TransactionResult.CashNotAvailable;
                }

                if (useSafeCashWithdrawal)
                {
                    fromAccount.Balance -= amount;
                    await transactionUnitOfWork.BankAccountRepository.UpdateAsync(fromAccount);
                    await transactionUnitOfWork.SaveChangesAsync();

                    if (!cashStorage.Withdraw(amount))
                    {
                        return TransactionResult.CashWithdrawalError;
                    }
                }
                else
                {
                    fromAccount.Balance -= amount;
                    await transactionUnitOfWork.BankAccountRepository.UpdateAsync(fromAccount);
                    if (cashStorage.Withdraw(amount))
                    {
                        await transactionUnitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        await transactionUnitOfWork.CancelAsync();
                    }
                }

                return TransactionResult.Success;
            }
            catch (Exception ex)
            {
                return new TransactionResult(TransactionResultCodes.Error, ex.Message);
            }
        }
    }
}
