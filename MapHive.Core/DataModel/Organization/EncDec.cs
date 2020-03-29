using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Npgsql;

namespace MapHive.Core.DataModel
{
    public partial class Organization
    {
        /// <summary>
        /// encrypts org dbs
        /// </summary>
        public void EncryptDatabases()
        {
            EncryptDatabases(GetEmptyKey());
        }

        /// <summary>
        /// encrypts org dbs
        /// </summary>
        /// <returns></returns>
        public void EncryptDatabases(object key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            var json = JsonConvert.SerializeObject(Databases ?? new List<OrganizationDatabase>());
            EncryptedDatabases = Cartomatic.Utils.Crypto.SymmetricEncryption.Encrypt(json, GetPrivateEncDecKey(key));
            Databases = null;
        }

        /// <summary>
        /// decrypts org dbs
        /// </summary>
        public void DecryptDatabases()
        {
            DecryptDatabases(GetEmptyKey());
        }

        /// <summary>
        /// decrypts org dbs
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public void DecryptDatabases(object key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrEmpty(EncryptedDatabases))
            {
                Databases = null;
            }
            else
            {
                var json = Cartomatic.Utils.Crypto.SymmetricEncryption.Decrypt(EncryptedDatabases, GetPrivateEncDecKey(key));
                Databases = JsonConvert.DeserializeObject<List<OrganizationDatabase>>(json);
            }
            EncryptedDatabases = null;
        }

        /// <summary>
        /// gets a private encryption key to secure sensitive db info; when a client decrypts it needs to know the private key too
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected string GetPrivateEncDecKey(object key)
        {
            var cfg = Cartomatic.Utils.NetCoreConfig.GetNetCoreConfig();
            var privateKey = cfg.GetValue("UserConfigurationEncryptionKey", string.Empty);
           
            return $"{Uuid}.{key}.{privateKey}";
        }

        /// <summary>
        /// Gets an 'empty' encryption key, when an external one is not provided
        /// </summary>
        /// <returns></returns>
        protected string GetEmptyKey()
        {
            return "empty";
        }
    }
}
