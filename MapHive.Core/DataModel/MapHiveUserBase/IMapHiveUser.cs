using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MapHive.Core.DataModel
{
    public interface IMapHiveUser : IBase
    {
        /// <summary>
        /// User's email. Email must be unique in the system and is also a username
        /// </summary>
        string Email { get; set; }

        /// <summary>
        /// Identity equivalent of locked account
        /// </summary>
        bool IsAccountClosed { get; set; }

        /// <summary>
        /// Identity equivalent of verified email
        /// </summary>
        bool IsAccountVerified { get; set; }
    }
}
