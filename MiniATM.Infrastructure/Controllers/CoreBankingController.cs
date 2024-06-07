using Microsoft.AspNetCore.Mvc;
using MiniATM.Infrastructure.Models;
using MiniATM.UseCase;

namespace MiniATM.Infrastructure.Controllers
{
    public class CoreBankingController : Controller
    {
        public readonly IBankAccountFinder accountFinder;
        private readonly ITransferManager transferManager;
        private readonly ICashWithdrawalManager cashWithdrawalManager;

        public CoreBankingController(IBankAccountFinder accountFinder, ITransferManager transferManager, ICashWithdrawalManager cashWithdrawalManager)
        {
            this.accountFinder = accountFinder;
            this.transferManager = transferManager;
            this.cashWithdrawalManager = cashWithdrawalManager;
        }

        private static Guid GetCustomerId()
        {
            return DemoConstants.CustomerId;
        }

        public async Task<IActionResult> ChooseBankAccountAsync(string returnUrl)
        {
            var customerId = GetCustomerId();
            var accounts = await accountFinder.FindByCustomerIdAsync(customerId) ?? [];

            return View(new ChooseAccountModel() { BankAccounts = accounts, ReturnUrl = returnUrl });
        }

        #region Transfer
        [HttpGet]
        public IActionResult Transfer(string bankAccount)
        {
            return View(new TransferModel() 
            { 
                FromBankAccount = bankAccount,
                ToBankAccount = string.Empty,
                Amount = 0,
            });
        }

        [HttpPost]
        public async Task<IActionResult> TransferAsync([FromForm]TransferModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var result = await transferManager.TransferAsync(model.FromBankAccount, model.ToBankAccount, model.Amount);

            return View("TransferResult", new TransferResultModel() { 
                FromBankAccount = model.FromBankAccount,
                ToBankAccount = model.ToBankAccount,
                Amount = model.Amount,
                ResultCode = result.ResultCode,
                Message = result.Message
            });
        }
        #endregion

        #region Withdrawal
        [HttpGet]
        public IActionResult WithDraw(string bankAccount)
        {
            return View(new WithdrawModel()
            {
                FromBankAccount = bankAccount,
                Amount = 0,
            });
        }
        public async Task<IActionResult> WithdrawAsync([FromForm] WithdrawModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var result = await cashWithdrawalManager.WithdrawAsync(model.FromBankAccount, model.Amount);

            return View("WithdrawResult", new WithdrawResultModel()
            {
                FromBankAccount = model.FromBankAccount,
                Amount = model.Amount,
                ResultCode = result.ResultCode,
                Message = result.Message
            });
        }
        #endregion
    }
}
