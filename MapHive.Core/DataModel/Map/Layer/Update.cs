using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DataModel.Map
{
    public partial class Layer
    {
        protected internal override async Task<T> UpdateAsync<T>(DbContext dbCtx, Guid uuid)
        {
            //need current version of the object in order to restore data source credentials
            var l = await base.ReadAsync<Layer>(dbCtx, uuid);

            if (l?.DataSource?.DataSourceCredentials != null)
                DataSource.DataSourceCredentials = l.DataSource.DataSourceCredentials;

            if (l?.SourceLayer?.DataSource?.DataSourceCredentials != null)
                SourceLayer.DataSource.DataSourceCredentials = l.SourceLayer.DataSource.DataSourceCredentials;

            return await base.UpdateAsync<T>(dbCtx, uuid);
        }
    }
}
