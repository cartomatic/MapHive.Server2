using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DataModel.Map
{
    public abstract partial class LayerBase
    {
        protected internal override async Task<T> UpdateAsync<T>(DbContext dbCtx, Guid uuid)
        {
            throw new Exception("Override or Use the UpdateAsync<TLayer, TDataSource> method instead!");
        }

        protected internal virtual async Task<TLayer> UpdateAsync<TLayer, TDataSource>(DbContext dbCtx, Guid uuid)
            where TLayer: LayerBase
            where TDataSource: DataSource
        {
            //need current version of the object in order to restore data source credentials
            var l = await base.ReadAsync<TLayer>(dbCtx, uuid);

            if (l?.DataSource?.DataSourceCredentials != null)
                DataSource.DataSourceCredentials = l.DataSource.DataSourceCredentials;

            //source layer is always by reference, so no need to restore anything!
            //if (l?.SourceLayer?.DataSource?.DataSourceCredentials != null)
            //    SourceLayer.DataSource.DataSourceCredentials = l.SourceLayer.DataSource.DataSourceCredentials;

            return await base.UpdateAsync<TLayer>(dbCtx, uuid);
        }
    }
}
