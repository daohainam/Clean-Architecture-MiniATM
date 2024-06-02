using AutoMapper;
using MiniATM.Infrastructure.SqlServer.Repositories.SqlServer.DataContext;
using MiniATM.UseCase.Repositories;
using MiniATM.UseCase.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniATM.Infrastructure.SqlServer.Repositories.SqlServer
{
    public class SqlServerTransactionUnitOfWork : ITransactionUnitOfWork
    {
        private readonly MiniATMContext context;
        private readonly IMapper mapper;

        public SqlServerTransactionUnitOfWork(MiniATMContext context, IMapper mapper)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            TransactionRepository = new SqlServerTransactionRepository(context, mapper);
            BankAccountRepository = new SqlServerBankAccountRepository(context, mapper);
        }

        public ITransactionRepository TransactionRepository { get; }

        public IBankAccountRepository BankAccountRepository { get; }

        public Task BeginTransactionAsync()
        {
            // we can use context.Database.BeginTransaction(), but since this is an UoW, we just silently discard changes if SaveChangesAsync is not called

            return Task.CompletedTask;
        }

        public Task CancelAsync()
        {
            // we can use transaction.Commit(), but since this is an UoW, we just silently discard changes if SaveChangesAsync is not called

            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
