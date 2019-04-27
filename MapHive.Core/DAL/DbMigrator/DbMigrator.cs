using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DAL
{
    public class DbMigrator
    {
        public static async Task MapHiveMetaDbMigrator(DbContext dbctx)
        {

            var dbFeedback = dbctx.CreateOrUpdateDatabase();

            if (dbFeedback.Created)
            {
                //nothing really, seed done the std map hive way via  CreateOrUpdateDatabase
            }

            if (dbFeedback.Updated)
            {
                //nothing really
            }
        }
    }
}
