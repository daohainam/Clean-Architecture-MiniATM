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
    public class TransactionManager(
        ITransactionUnitOfWork transactionUnitOfWork,
        ICashStorage cashStorage,
        bool useSafeCashWithdrawal = true
            ) : ITransactionManager
    {
        private readonly ITransactionUnitOfWork transactionUnitOfWork = transactionUnitOfWork ?? throw new ArgumentNullException(nameof(transactionUnitOfWork));
        private readonly ICashStorage cashStorage = cashStorage ?? throw new ArgumentNullException(nameof(cashStorage));

        public async Task<TransactionResult> TransferAsync(string fromAccountId, string toAccountId, double amount)
        {
            try
            {
                await transactionUnitOfWork.BeginTransactionAsync();

                var fromAccount = await transactionUnitOfWork.BankAccountRepository.FindByIdAsync(fromAccountId);
                if (fromAccount == null || fromAccount.IsLocked)
                {
                    return TransactionResult.SourceNotFound;
                }

                var balanceLeft = fromAccount.Balance - amount;
                if (balanceLeft < fromAccount.MinimumRequiredAmount)
                {
                    return TransactionResult.BalanceTooLow;
                }

                var toAccount = await transactionUnitOfWork.BankAccountRepository.FindByIdAsync(toAccountId);
                if (toAccount == null || toAccount.IsLocked)
                {
                    return TransactionResult.DestinationNotFound;
                }

                fromAccount.Balance -= amount;
                await transactionUnitOfWork.BankAccountRepository.UpdateAsync(fromAccount);

                var now = DateTime.UtcNow;
                await transactionUnitOfWork.TransactionRepository.Add(new Transaction()
                {
                    Id = Guid.NewGuid(),
                    Amount = amount,
                    AccountId = fromAccount.Id,
                    DateTimeUTC = now,
                    TransactionTypes = TransactionTypes.Withdrawal,
                    Notes = $"Transfer to {toAccount.Id}"
                });

                toAccount.Balance -= amount;
                await transactionUnitOfWork.BankAccountRepository.UpdateAsync(toAccount);
                await transactionUnitOfWork.TransactionRepository.Add(new Transaction()
                {
                    Id = Guid.NewGuid(),
                    Amount = amount,
                    AccountId = toAccount.Id,
                    DateTimeUTC = now,
                    TransactionTypes = TransactionTypes.Deposit,
                    Notes = $"Transfer from {fromAccount.Id}"
                });

                await transactionUnitOfWork.SaveChangesAsync();

                return TransactionResult.Success;
            }
            catch (Exception ex)
            {
                return new TransactionResult(TransactionResultCodes.Error, ex.Message);
            }
        }

        public async Task<TransactionResult> WithdrawCashAsync(string accountId, double amount)
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
