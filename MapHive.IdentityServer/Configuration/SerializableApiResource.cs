﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cartomatic.Utils.Dto;
using IdentityServer4.Models;

namespace MapHive.IdentityServer.Configuration
{
    /// <summary>
    /// SerializableApiResource hides base interface type properties of ApiResource with their implementation, so can deserialize object
    /// </summary>
    public class SerializableApiResource : ApiResource
    {
        //public bool Enabled {get; set; }
        //public string Name { get; set; }
        //public string DisplayName { get; set; }
        //public string Description { get; set; }

        /// <summary>
        /// List of accociated user claims that should be included when this resource is requested.
        /// </summary>
        public new List<string> UserClaims { get; set; }

        /// <summary>
        /// The API secret is used for the introspection endpoint. The API can authenticate
        /// with introspection using the API name and secret.
        /// </summary>
        public new List<Secret> ApiSecrets { get; set; }

        /// <summary>
        /// An API must have at least one scope. Each scope can have different settings.
        /// </summary>
        public new List<SerializableScope> Scopes { get; set; }

        public ApiResource ToApiResource()
        {
            var apiResource = this.CopyPublicPropertiesToNew<ApiResource>();

            //hidden properties need to be copied explicitly as dto utils will not matche them on type!
            apiResource.UserClaims = UserClaims;
            apiResource.ApiSecrets = ApiSecrets.Select(cs => new Secret(cs.Value.Sha256(), cs.Description, cs.Expiration)).ToList();
            apiResource.Scopes = Scopes.Select(s=>s.ToScope()).ToList();

            return apiResource;
        }
    }
}
