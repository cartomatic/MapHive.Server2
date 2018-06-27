using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapHive.Core
{
    /// <summary>
    /// account creation output data
    /// </summary>
    public class AccountCreateOutput
    {
        public string InitialPassword { get; set; }


        public string VerificationKey { get; set; }

        /// <summary>
        /// Email template used during the account creation procedure; all the tokens generated during the account creation should already be applied to the template
        /// </summary>
        public Cartomatic.Utils.Email.EmailTemplate EmailTemplate { get; set; }
    }
}
