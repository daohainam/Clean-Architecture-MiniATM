using MiniATM.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniATM.UseCase.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer?> FindByIdAsync(Guid id);
    }
}
