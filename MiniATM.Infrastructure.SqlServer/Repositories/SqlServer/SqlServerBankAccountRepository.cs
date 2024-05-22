using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MiniATM.Entities;
using MiniATM.Infrastructure.SqlServer.Repositories.SqlServer.DataContext;
using MiniATM.UseCase.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniATM.Infrastructure.SqlServer.Repositories.SqlServer
{
    public class SqlServerBankAccountRepository : IBankAccountRepository
    {
        private readonly MiniATMContext context;
        private readonly IMapper mapper;

        public SqlServerBankAccountRepository(MiniATMContext context, IMapper mapper)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<Entities.BankAccount>> FindByCustomerIdAsync(Guid customerId)
        {
            var dbaccounts = await context.BankAccounts.Where(ba => ba.CustomerId == customerId).ToListAsync();

            return mapper.Map<IEnumerable<Entities.BankAccount>>(dbaccounts);
        }

        public async Task<Entities.BankAccount?> FindByIdAsync(string accountId)
        {
            var dbaccount = await context.BankAccounts.Where(ba => ba.Id == accountId).FirstOrDefaultAsync();

            return mapper.Map<Entities.BankAccount>(dbaccount);
        }

        public async Task UpdateAsync(Entities.BankAccount account)
        {
            var dbaccount = await context.BankAccounts.Where(ba => ba.Id == account.Id).FirstOrDefaultAsync();

            if (dbaccount != null)
            {
                mapper.Map(account, dbaccount);
            }

            await context.SaveChangesAsync();
        }
    }
}
