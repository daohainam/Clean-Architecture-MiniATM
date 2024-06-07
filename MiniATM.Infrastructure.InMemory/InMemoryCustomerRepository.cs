using MiniATM.Entities;
using MiniATM.UseCase.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniATM.Infrastructure.InMemory
{
    public class InMemoryCustomerRepository: ICustomerRepository
    {
        private readonly List<Customer> customers;

        public InMemoryCustomerRepository()
        {
            customers =
            [
                new Customer() { 
                    Id = Guid.Empty,
                    Name = "Nam.NET" 
                },
            ];
        }

        public InMemoryCustomerRepository(List<Customer> initCustomers) { 
            customers = initCustomers;
        }

        public Task<Customer?> FindByIdAsync(Guid id)
        {
            return Task.FromResult(customers.Where(c => c.Id == id).SingleOrDefault());
        }
    }
}
