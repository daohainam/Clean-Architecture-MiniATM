using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniATM.UseCase.Caching
{
    public class CachableBankAccountFinderOptions
    {
        public string CacheKey { get; set; } = "CUSTOMER|ID:";
        public TimeSpan CacheTimeSpan { get; set; } = TimeSpan.FromSeconds(15);
    }
}
