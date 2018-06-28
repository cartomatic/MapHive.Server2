using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapHive.Core.DataModel
{
    public abstract partial class MapHiveUserBase
    {
        /// <summary>
        /// User's email. Email must be unique in the system and is also a username
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Identity equivalent of a locked account
        /// </summary>
        public bool IsAccountClosed { get; set; }

        /// <summary>
        /// Identity equivalent of email confirmed
        /// </summary>
        public bool IsAccountVerified { get; set; }
    }
}
