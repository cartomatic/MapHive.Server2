using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapHive.Core
{
    /// <summary>
    /// Input gathered from a client in order to create an account
    /// </summary>
    public class AccountCreateDetails
    {
        /// <summary>
        /// User email to create an account for
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Surname of a user
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Forename
        /// </summary>
        public string Forename { get; set; }

        /// <summary>
        /// phone number a user can be contacted at
        /// </summary>
        public string ContactPhone { get; set; }

        /// <summary>
        /// Company to create an account for
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// VAT number
        /// </summary>
        public string VatNumber { get; set; }

        /// <summary>
        /// Address street
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// Address house no
        /// </summary>
        public string HouseNo { get; set; }

        /// <summary>
        /// Address flat no
        /// </summary>
        public string FlatNo { get; set; }

        /// <summary>
        /// Address postcode
        /// </summary>
        public string Postcode { get; set; }

        /// <summary>
        /// Address City
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Address city
        /// </summary>
        public string Country { get; set; }
    }
}
