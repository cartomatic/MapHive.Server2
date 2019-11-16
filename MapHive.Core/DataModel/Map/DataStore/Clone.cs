using Cartomatic.Utils.Dto;
using System.Collections.Generic;
using System.Linq;

namespace MapHive.Core.DataModel.Map
{
    public partial class DataStore
    {
        public DataStore Clone(bool resetCols = false)
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
