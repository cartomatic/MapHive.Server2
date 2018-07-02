using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cartomatic.Utils.Dto;
using IdentityServer4.Models;

namespace MapHive.IdentityServer.Configuration
{
    /// <summary>
    /// SerializableScope hides base interface type properties of Scope with their implementation, so can deserialize object
    /// </summary>
    public class SerializableScope : Scope
    {
        /// <summary>
        /// List of user-claim types that should be included in the access token.
        /// </summary>
        public new List<string> UserClaims { get; set; }

        public Scope ToScope()
        {
            var scope = this.CopyPublicPropertiesToNew<Scope>();

            //hidden properties need to be copied explicitly as dto utils will not matche them on type!
            scope.UserClaims = UserClaims;

            return scope;
        }
    }
}
