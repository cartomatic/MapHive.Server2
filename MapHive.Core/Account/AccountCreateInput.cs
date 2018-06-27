using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DataModel;

namespace MapHive.Core
{
    public class AccountCreateInput
    {
        /// <summary>
        /// The actual details provided by a client
        /// </summary>
        public AccountCreateDetails AccountDetails { get; set; }

        /// <summary>
        /// Email template to be used during the account creation procedure
        /// </summary>
        public Cartomatic.Utils.Email.EmailTemplate EmailTemplate { get; set; }

        /// <summary>
        /// email account details to be used when sending out emails by the account creation api; when not provided, email will not be sent
        /// </summary>
        public Cartomatic.Utils.Email.EmailAccount EmailAccount { get; set; }

        /// <summary>
        /// License opts to be applied for the org for each registered application; keys are app shortnames;
        /// apps specified here will be connected to created organisation with the opts specified;
        /// if default license opts are to be applied, simply add a key with a null for the opts
        /// </summary>
        public Dictionary<string, LicenseOptions> LicenseOptions { get; set; }

        
    }
}
