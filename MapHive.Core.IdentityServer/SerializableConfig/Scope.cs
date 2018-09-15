using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cartomatic.Utils.Dto;
using IdentityModel;
using IdentityServer4.Models;

namespace MapHive.Core.IdentityServer.SerializableConfig
{
    /// <summary>
    /// SerializableScope hides base interface type properties of Scope with their implementation, so can deserialize object
    /// </summary>
    public class Scope : IdentityServer4.Models.Scope
    {
        /// <summary>
        /// List of user-claim types that should be included in the access token in addition to JwtClaimTypes.Subject - subject id
        /// See JwtClaimTypes for details
        /// </summary>
        public new List<string> UserClaims { get; set; }

        public IdentityServer4.Models.Scope ToScope()
        {
            var scope = (this).CopyPublicPropertiesToNew<IdentityServer4.Models.Scope>();

            //hidden properties need to be copied explicitly as dto utils will not matche them on type!
            scope.UserClaims = UserClaims;
            
            return scope;
        }
    }
}
