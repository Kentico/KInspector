namespace Kentico.KInspector.Core
{
    /// <summary>
    /// Result's status flag.
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// Used in cases when displaying general information. 
        /// </summary>
        Info = 0,


        /// <summary>
        /// Indicates issues without any problems.
        /// </summary>
        Good = 1,


        /// <summary>
        /// Indicates issue that may be a problem.
        /// </summary>
        Warning = 2,


        /// <summary>
        /// Indicates error that needs to be fixed.
        /// </summary>
        Error = 3
    }
}