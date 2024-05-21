using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniATM.Entities
{
    public class Customer
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
    }
}
