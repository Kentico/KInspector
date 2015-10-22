namespace Kentico.KInspector.Core
{
    /// <summary>
    /// Indicates WHAT does the <see cref="IModule"/> should return in <see cref="ModuleResults"/>.
    /// </summary>
    public enum ModuleResultsType
    {
        /// <summary>
        /// This result type will be listed as a normal sentence in the UI.
        /// </summary>
        String,


        /// <summary>
        /// This result type will be displayed as a unordered list in the UI.
        /// </summary>
        List,


        /// <summary>
        /// This result type will be displayed as a table.
        /// Can be used for <see cref="System.Data.DataTable"/>
        /// </summary>
        Table,


        /// <summary>
        /// This result type will be displayed as a list of tables.
        /// Can be used for <see cref="List{System.Data.DataTable}"/> or <see cref="System.Data.DataSet"/>
        /// </summary>
        ListOfTables
    }
}
