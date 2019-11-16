using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cartomatic.Utils.Dto;

namespace MapHive.Core.DataModel.Map
{
    /// <summary>
    /// a unified db data source that could have been imported into db from shp, geoJson, csv, service, etc
    /// </summary>
    public partial class DataStore : DataStoreBase
    {
        static DataStore()
        {
            BaseObjectTypeIdentifierExtensions.RegisterTypeIdentifier(MethodInfo.GetCurrentMethod().DeclaringType, Guid.Parse("d84701ad-913f-4e99-9ed7-749230f132a5"));
        }

        public override DataStoreBase Clone(bool resetCols = false)
        {
            return new DataStore
            {
                Name = Name,
                DataSource = new DataSource
                {
                    DataSourceCredentials = DataSource.DataSourceCredentials.CopyPublicPropertiesToNew<Cartomatic.Utils.Data.DataSourceCredentials>(),
                    Schema = DataSource.Schema,
                    Table = DataSource.Table,
                    Columns = resetCols
                        ? new List<Column>()
                        : DataSource.Columns.ToList() //so a new container is created
                }
            };
        }
    }
}
