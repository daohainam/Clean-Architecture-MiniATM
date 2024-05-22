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
    public class TransferManager(
        ITransactionUnitOfWork transactionUnitOfWork
            ) : ITransferManager
    {
        private readonly ITransactionUnitOfWork transactionUnitOfWork = transactionUnitOfWork ?? throw new ArgumentNullException(nameof(transactionUnitOfWork));

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

                toAccount.Balance += amount;
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
    }
}
