using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniATM.Infrastructure.SqlServer.Repositories.SqlServer.DataContext
{
    public class Customer
    {
        public Guid Id { get; set; }
        [MaxLength(50)]
        public required string Name { get; set; }
    }
}
