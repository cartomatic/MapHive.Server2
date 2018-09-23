namespace MapHive.Core.DataModel.Dictionary
{
    /// <summary>
    /// enforces name / desc properties to be present
    /// </summary>
    public interface ISimpleDictionary
    {
        /// <summary>
        /// object name
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Object description
        /// </summary>
        string Description { get; set; }
    }
}