using MiniATM.UseCase;

namespace MiniATM.Infrastructure.CashStorage
{
    public class InMemoryCashStorage : ICashStorage
    {
        private readonly ILogger<InMemoryCashStorage> logger;
        private double availableAmount;
        private SpinLock spinLock = new();

        public InMemoryCashStorage(ILogger<InMemoryCashStorage> logger, double initAmount) {
            this.logger = logger;
            availableAmount = initAmount;

            logger.LogInformation("CashStorage loaded with amount = {a})", availableAmount);
        }

        public bool IsCashAmountAvailable(double amount)
        {
            bool available = availableAmount >= amount;
            logger.LogInformation("IsCashAmountAvailable({a}): {av} (current amount: {am})", amount, available, availableAmount);

            return available;
        }

        public bool Withdraw(double amount)
        {
            bool gotLock = false;
            bool succeeded = false;
            try
            {
                spinLock.Enter(ref gotLock);

                if (availableAmount >= amount)
                { 
                    availableAmount -= amount;
                    succeeded = true;
                }
            }
            finally
            {
                if (gotLock) spinLock.Exit();
            }

            logger.LogInformation("Withdraw({a}) result: {s} (current amount: {am})", amount, succeeded, availableAmount);

            return succeeded;
        }
    }
}
