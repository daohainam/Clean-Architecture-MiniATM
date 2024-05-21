using MiniATM.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniATM.UseCase.Repositories
{
    public interface IBankAccountRepository
    {
        Task<BankAccount?> FindByIdAsync(string accountId);
        Task<IEnumerable<BankAccount>> FindByCustomerIdAsync(Guid customerId);
        Task UpdateAsync(BankAccount fromAccount);
    }
}
