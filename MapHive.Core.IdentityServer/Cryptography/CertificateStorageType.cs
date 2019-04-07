using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MapHive.Core.IdentityServer
{
    /// <summary>
    /// Certificate storage type
    /// </summary>
    public enum CertificateStorageType
    {
#pragma warning disable 1591
        File,
        Embedded,
        Store
    }
}