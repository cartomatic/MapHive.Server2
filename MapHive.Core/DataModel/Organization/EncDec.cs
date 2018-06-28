using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Npgsql;

namespace MapHive.Core.DataModel
{
    public partial class Organization
    {
        /// <summary>
        /// encrypts org dbs
        /// </summary>
        /// <returns></returns>
        public void EncryptDatabases(object key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            var json = JsonConvert.SerializeObject(Databases ?? new List<OrganizationDatabase>());
            EncryptedDatabases = Cartomatic.Utils.Crypto.SymmetricEncryption.Encrypt(json, GetEncDecKey(key));
            Databases = null;
        }

        /// <summary>
        /// decrypts org dbs
        /// </summary>
        /// <param name="encrypted"></param>
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
                var json = Cartomatic.Utils.Crypto.SymmetricEncryption.Decrypt(EncryptedDatabases, GetEncDecKey(key));
                Databases = JsonConvert.DeserializeObject<List<OrganizationDatabase>>(json);
            }
            EncryptedDatabases = null;
        }

        //gets an encryption key
        protected string GetEncDecKey(object key)
        {
            return $"{Uuid}.{key}";
        }
    }
}
