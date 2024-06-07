using MiniATM.UseCase;
using MiniATM.UseCase.Repositories;
using MiniATM.UseCase.UnitOfWork;
using Moq;

namespace UseCaseTests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1031:Do not use blocking task operations in test method", Justification = "<Pending>")]
    public class TransferTests
    {
        [Fact]
        public async Task BalanceTooLow_TestAsync()
        {
            var transactionRepository = new Mock<ITransactionRepository>();

            var bankAccountRepository = new Mock<IBankAccountRepository>();
            bankAccountRepository.Setup(o => o.FindByIdAsync("0001").Result).Returns(new MiniATM.Entities.BankAccount()
            {
                Balance = 1000,
                Currency = "VND",
                CustomerId = Guid.Empty,
                Id = "0001",
                IsLocked = false,
                MinimumRequiredAmount = 0
            });

            bankAccountRepository.Setup(o => o.FindByIdAsync("0002").Result).Returns(new MiniATM.Entities.BankAccount()
            {
                Balance = 0,
                Currency = "VND",
                CustomerId = Guid.Empty,
                Id = "0002",
                IsLocked = false,
                MinimumRequiredAmount = 0
            });

            var transactionUnitOfWork = new SimpleTransactionUnitOfWork(transactionRepository.Object, bankAccountRepository.Object);

            var transactionManager = new TransferManager(transactionUnitOfWork);

            var result = await transactionManager.TransferAsync("0001", "0002", 2000);
            Assert.Equal(TransactionResultCodes.BalanceTooLow, result.ResultCode);
        }

        [Fact]
        public async Task Success_TestAsync()
        {
            var transactionRepository = new Mock<ITransactionRepository>();

            var bankAccountRepository = new Mock<IBankAccountRepository>();
            bankAccountRepository.Setup(o => o.FindByIdAsync("0001").Result).Returns(new MiniATM.Entities.BankAccount()
            {
                Balance = 3000,
                Currency = "VND",
                CustomerId = Guid.Empty,
                Id = "0001",
                IsLocked = false,
                MinimumRequiredAmount = 0
            });

            bankAccountRepository.Setup(o => o.FindByIdAsync("0002").Result).Returns(new MiniATM.Entities.BankAccount()
            {
                Balance = 0,
                Currency = "VND",
                CustomerId = Guid.Empty,
                Id = "0002",
                IsLocked = false,
                MinimumRequiredAmount = 0
            });

            var transactionUnitOfWork = new SimpleTransactionUnitOfWork(transactionRepository.Object, bankAccountRepository.Object);

            var transactionManager = new TransferManager(transactionUnitOfWork);

            var result = await transactionManager.TransferAsync("0001", "0002", 2000);
            Assert.Equal(TransactionResultCodes.Success, result.ResultCode);
            Assert.Equal(1000, transactionUnitOfWork.BankAccountRepository.FindByIdAsync("0001").Result?.Balance);
            Assert.Equal(2000, transactionUnitOfWork.BankAccountRepository.FindByIdAsync("0002").Result?.Balance);
        }

        [Fact]
        public async Task SendFromLockedAccount_TestAsync()
        {
            var transactionRepository = new Mock<ITransactionRepository>();

            var bankAccountRepository = new Mock<IBankAccountRepository>();
            bankAccountRepository.Setup(o => o.FindByIdAsync("0001").Result).Returns(new MiniATM.Entities.BankAccount()
            {
                Balance = 3000,
                Currency = "VND",
                CustomerId = Guid.Empty,
                Id = "0001",
                IsLocked = true,
                MinimumRequiredAmount = 0
            });

            bankAccountRepository.Setup(o => o.FindByIdAsync("0002").Result).Returns(new MiniATM.Entities.BankAccount()
            {
                Balance = 0,
                Currency = "VND",
                CustomerId = Guid.Empty,
                Id = "0002",
                IsLocked = false,
                MinimumRequiredAmount = 0
            });

            var transactionUnitOfWork = new SimpleTransactionUnitOfWork(transactionRepository.Object, bankAccountRepository.Object);

            var transactionManager = new TransferManager(transactionUnitOfWork);

            var result = await transactionManager.TransferAsync("0001", "0002", 2000);
            Assert.Equal(TransactionResultCodes.SourceNotFound, result.ResultCode);
        }

        [Fact]
        public async Task NegativeAccount_TestAsync()
        {
            var transactionRepository = new Mock<ITransactionRepository>();

            var bankAccountRepository = new Mock<IBankAccountRepository>();
            bankAccountRepository.Setup(o => o.FindByIdAsync("0001").Result).Returns(new MiniATM.Entities.BankAccount()
            {
                Balance = 3000,
                Currency = "VND",
                CustomerId = Guid.Empty,
                Id = "0001",
                IsLocked = false,
                MinimumRequiredAmount = -8000
            });

            bankAccountRepository.Setup(o => o.FindByIdAsync("0002").Result).Returns(new MiniATM.Entities.BankAccount()
            {
                Balance = 3000,
                Currency = "VND",
                CustomerId = Guid.Empty,
                Id = "0002",
                IsLocked = false,
                MinimumRequiredAmount = 0
            });

            var transactionUnitOfWork = new SimpleTransactionUnitOfWork(transactionRepository.Object, bankAccountRepository.Object);

            var transactionManager = new TransferManager(transactionUnitOfWork);

            var result = await transactionManager.TransferAsync("0001", "0002", 5000);
            Assert.Equal(TransactionResultCodes.Success, result.ResultCode);
            Assert.Equal(-2000, transactionUnitOfWork.BankAccountRepository.FindByIdAsync("0001").Result?.Balance);
            Assert.Equal(8000, transactionUnitOfWork.BankAccountRepository.FindByIdAsync("0002").Result?.Balance);
        }
    }
}