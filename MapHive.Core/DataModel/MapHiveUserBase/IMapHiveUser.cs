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
