using System;
using System.Collections.Generic;
using System.Text;
using Cartomatic.Utils.Ef;
using MapHive.Core.DataModel;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DAL
{
    public partial class ExtendedViews
    {
        public static void CreateView_TranslationKeyExtended(MapHiveDbContext dbCtx)
        {
            //init som objects so we can revers engineer some db names off them
            var tk = new TranslationKey();
            var tkExt = new TranslationKeyExtended();
            var lc = new LocalizationClass();
            


            //all the objects are kept in the same schema
            var schema = dbCtx.GetTableSchema(tk);

            //view name to create in the db
            var viewName = TranslationKeyExtended.ViewName;

            
            //Note:
            //basically we should make sure the view gets recreated when its definition has changed.
            //so such logic may have to be implemented at some stage in order to avoid recreating the view on each seed.
            //on the other hand, for moderate amounts of data dropping and recreating the view should not be a problem

            //also, no db objects should really depend on this view, because this could tie us hard.
            //therefore dropping cascade seems like a quite good idea ;)


            //try drop table - this will work on the first go when a table generated via a migration is present and fail on subsequent runs when there is no table anymore
            try
            {
                var dropTableSql = $@"DROP TABLE IF EXISTS ""{schema}"".""{viewName}"" CASCADE;";
                dbCtx.Database.ExecuteSqlCommand(dropTableSql);
            }
            catch
            {
                //ignore
            }

            var dropViewSql = $@"DROP VIEW IF EXISTS ""{schema}"".""{viewName}"" CASCADE;";
            dbCtx.Database.ExecuteSqlCommand(dropViewSql);



            //Note:
            //because of some reason ExecuteSqlCommand when passed a variable executes sql as expected
            //but when txt is assembled within the method call it gets auto parametrised. perhaps $@"" string has different type and is treated differently...
            //dunno.


            //Create view sql
            var viewSql = $@"
CREATE VIEW ""{schema}"".""{viewName}"" AS
SELECT
    tk.*,
    lc.{dbCtx.GetTableColumnName(lc, nameof(LocalizationClass.ApplicationName))} as {dbCtx.GetTableColumnName(tkExt, nameof(TranslationKeyExtended.ApplicationName))},
    lc.{dbCtx.GetTableColumnName(lc, nameof(LocalizationClass.ClassName))} as {dbCtx.GetTableColumnName(tkExt, nameof(TranslationKeyExtended.ClassName))},
    lc.{dbCtx.GetTableColumnName(lc, nameof(LocalizationClass.InheritedClassName))} as {dbCtx.GetTableColumnName(tkExt, nameof(TranslationKeyExtended.InheritedClassName))}
FROM
    ""{schema}"".""{dbCtx.GetTableName(tk)}"" tk
    left outer join ""{schema}"".""{dbCtx.GetTableName(lc)}"" lc on lc.{dbCtx.GetTableColumnName(lc, nameof(LocalizationClass.Uuid))} = tk.{dbCtx.GetTableColumnName(tk, nameof(TranslationKey.LocalizationClassUuid))}
;
";

            dbCtx.Database.ExecuteSqlCommand(viewSql);

        }
    }
}
