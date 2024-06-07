using MiniATM.Entities;
using MiniATM.UseCase.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniATM.UseCase
{
    public class RepositoryBankAccountFinder(IBankAccountRepository bankAccountRepository): IBankAccountFinder
    {
        private readonly IBankAccountRepository bankAccountRepository = bankAccountRepository;

        public async Task<IEnumerable<BankAccount>> FindByCustomerIdAsync(Guid customerId)
        {
            return await bankAccountRepository.FindByCustomerIdAsync(customerId);
        }
    }
}
