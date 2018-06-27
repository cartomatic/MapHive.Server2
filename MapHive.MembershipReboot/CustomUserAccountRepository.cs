using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrockAllen.MembershipReboot.Ef;

namespace MapHive.MembershipReboot
{
    public class CustomUserAccountRepository : DbContextUserAccountRepository<CustomDbContext, CustomUserAccount>
    {
        public CustomUserAccountRepository(CustomDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
