using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniATM.UseCase
{
    public class TransactionResult
    {
        public TransactionResult(TransactionResultCodes resultCode, string message)
        {
            ResultCode = resultCode;
            Message = message ?? string.Empty;
        }

        public TransactionResultCodes ResultCode { get; }
        public string Message { get; }

        public static readonly TransactionResult SourceNotFound = new(TransactionResultCodes.SourceNotFound, string.Empty);
        public static readonly TransactionResult DestinationNotFound = new(TransactionResultCodes.DestinationNotFound, string.Empty);
        public static readonly TransactionResult BalanceTooLow = new(TransactionResultCodes.BalanceTooLow, string.Empty);
        public static readonly TransactionResult CashNotAvailable = new(TransactionResultCodes.CashNotAvailable, string.Empty);
        public static readonly TransactionResult CashWithdrawalError = new(TransactionResultCodes.CashWithdrawalError, string.Empty);
        public static readonly TransactionResult Success = new(TransactionResultCodes.Success, string.Empty);
    }
}
