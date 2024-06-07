using MiniATM.UseCase;

namespace MiniATM.Infrastructure.Models
{
    public class TransferResultModel: TransferModel
    {
        public required TransactionResultCodes ResultCode { get; set; }
        public required string Message { get; set; }
    }
}
