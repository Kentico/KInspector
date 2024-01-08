namespace KenticoInspector.Core.Constants
{
    public enum ResultsStatus
    {
        /// <summary>
        /// Used in cases when displaying general information.
        /// </summary>
        Information = 0,

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
        Error = 3,

        /// <summary>
        /// Indicates that the report has not been run
        /// </summary>
        NotRun = 4
    }
}