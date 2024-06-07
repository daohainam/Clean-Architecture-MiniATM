using MiniATM.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniATM.UseCase
{
    public interface IBankAccountFinder
    {
        Task<IEnumerable<BankAccount>> FindByCustomerIdAsync(Guid customerId);
    }
}
