﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if NETFULL
using System.Data.Entity;
#endif
#if NETSTANDARD
using Microsoft.EntityFrameworkCore;
#endif


namespace MapHive.Core.DataModel
{
    public partial class Organization
    {
        protected internal override async Task<T> CreateAsync<T>(DbContext dbCtx)
        {
            var org = await base.CreateAsync<T>(dbCtx) as Organization;

            //once an org is present need to create the default roles and link them with an org
            var rOwner = await CreateRoleAsync(dbCtx, OrgRoleIdentifierOwner, OrgRoleNameOwner);
            var rAdmin = await CreateRoleAsync(dbCtx, OrgRoleIdentifierAdmin, OrgRoleNameAdmin);
            var rMember = await CreateRoleAsync(dbCtx, OrgRoleIdentifierMember, OrgRoleNameMember);

            //link roles to org
            org.AddLink(rOwner);
            org.AddLink(rAdmin);
            org.AddLink(rMember);

            //and update the org
            await org.UpdateAsync(dbCtx);

            return (T)(Base)org;
        }
    }
}
