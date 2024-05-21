using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniATM.UseCase
{
    public interface ICashStorage
    {
        bool IsCashAmountAvailable(double amount);
        bool Withdraw(double amount);
    }
}
