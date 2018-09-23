namespace MapHive.Core.DataModel.Dictionary
{
    /// <summary>
    /// Enforces a presence of an identifier field
    /// </summary>
    public interface IIdentifierDictionary : ISimpleDictionary
    {
        /// <summary>
        /// Identifier
        /// </summary>
        string Identifier { get; set; }
    }
}