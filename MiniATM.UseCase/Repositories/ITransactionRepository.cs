using MiniATM.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniATM.UseCase.Repositories
{
    public interface ITransactionRepository
    {
        Task Add(Transaction transaction);
    }
}
