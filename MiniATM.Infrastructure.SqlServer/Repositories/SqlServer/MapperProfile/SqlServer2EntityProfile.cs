using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniATM.Infrastructure.SqlServer.Repositories.SqlServer.MapperProfile
{
    public class SqlServer2EntityProfile: Profile
    {
        public SqlServer2EntityProfile() {
            CreateMap<DataContext.Customer, Entities.Customer>();

            CreateMap<DataContext.BankAccount, Entities.BankAccount>();
            CreateMap<Entities.BankAccount, DataContext.BankAccount>();

            CreateMap<Entities.Transaction, DataContext.Transaction>();
        }
    }
}
