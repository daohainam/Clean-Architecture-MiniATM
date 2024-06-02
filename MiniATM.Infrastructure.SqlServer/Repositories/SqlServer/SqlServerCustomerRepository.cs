using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
    public class SqlServerCustomerRepository : ICustomerRepository
    {
        private readonly MiniATMContext context;
        private readonly IMapper mapper;

        public SqlServerCustomerRepository(MiniATMContext context, IMapper mapper)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Entities.Customer?> FindByIdAsync(Guid customerId)
        {
            var dbcustomer = await context.Customers.Where(ba => ba.Id == customerId).FirstOrDefaultAsync();

            return mapper.Map<Entities.Customer>(dbcustomer);
        }
    }
}
