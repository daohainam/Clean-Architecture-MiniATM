﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniATM.UseCase
{
    public interface ICashWithdrawalManager
    {
        Task<TransactionResult> WithdrawAsync(string accountId, double amount);
    }
}
