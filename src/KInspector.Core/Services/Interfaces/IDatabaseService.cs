using System.Data;

using KInspector.Core.Models;

namespace KInspector.Core.Services.Interfaces
{
    /// <summary>
    /// Contains methods for loading KInspector script files and executing SQL queries against a database.
    /// </summary>
    public interface IDatabaseService : IService
    {
        /// <summary>
        /// Connects to the database provided by the database settings.
        /// </summary>
        void Configure(DatabaseSettings databaseSettings);

        /// <summary>
        /// Executes SQL contained in a KInspector script file.
        /// </summary>
        /// <param name="relativeFilePath">The relative path of the .sql file to execute.</param>
        /// <param name="parameters">Values which will be provided as parameters in the SQL query.</param>
        Task ExecuteNonQuery(string relativeFilePath, dynamic parameters);

        /// <summary>
        /// Executes SQL contained in a KInspector script file and returns the results.
        /// </summary>
        /// <param name="relativeFilePath">The relative path of the .sql file to execute.</param>
        Task<IEnumerable<T>> ExecuteSqlFromFile<T>(string relativeFilePath);

        /// <summary>
        /// Executes SQL contained in a KInspector script file and returns the results.
        /// </summary>
        /// <param name="relativeFilePath">The relative path of the .sql file to execute.</param>
        /// <param name="parameters">Values which will be provided as parameters in the SQL query.</param>
        Task<IEnumerable<T>> ExecuteSqlFromFile<T>(string relativeFilePath, dynamic parameters);

        /// <summary>
        /// Executes SQL contained in a KInspector script file and returns the results.
        /// </summary>
        /// <param name="relativeFilePath">The relative path of the .sql file to execute.</param>
        /// <param name="literalReplacements">Text within the .sql file matching the keys of this dictionary will be replaced by the values before execution.</param>
        Task<IEnumerable<T>> ExecuteSqlFromFile<T>(string relativeFilePath, IDictionary<string, string> literalReplacements);

        /// <summary>
        /// Executes SQL contained in a KInspector script file and returns the results.
        /// </summary>
        /// <param name="relativeFilePath">The relative path of the .sql file to execute.</param>
        /// <param name="literalReplacements">Text within the .sql file matching the keys of this dictionary will be replaced by the values before execution.</param>
        /// <param name="parameters">Values which will be provided as parameters in the SQL query.</param>
        Task<IEnumerable<T>> ExecuteSqlFromFile<T>(string relativeFilePath, IDictionary<string, string> literalReplacements, dynamic parameters);

        /// <summary>
        /// Executes SQL contained in a KInspector script file and returns the results.
        /// </summary>
        /// <param name="relativeFilePath">The relative path of the .sql file to execute.</param>
        [Obsolete("A last resort when it is impossible to create a data model or use one of the generic options.")]
        Task<DataTable> ExecuteSqlFromFileAsDataTable(string relativeFilePath);

        /// <summary>
        /// Executes SQL contained in a KInspector script file and returns the results.
        /// </summary>
        /// <param name="relativeFilePath">The relative path of the .sql file to execute.</param>
        Task<IEnumerable<IDictionary<string, object>>> ExecuteSqlFromFileGeneric(string relativeFilePath);

        /// <summary>
        /// Executes SQL contained in a KInspector script file and returns the results.
        /// </summary>
        /// <param name="relativeFilePath">The relative path of the .sql file to execute.</param>
        /// <param name="parameters">Values which will be provided as parameters in the SQL query.</param>
        Task<IEnumerable<IDictionary<string, object>>> ExecuteSqlFromFileGeneric(string relativeFilePath, dynamic parameters);

        /// <summary>
        /// Executes SQL contained in a KInspector script file and returns the results.
        /// </summary>
        /// <param name="relativeFilePath">The relative path of the .sql file to execute.</param>
        /// <param name="literalReplacements">Text within the .sql file matching the keys of this dictionary will be replaced by the values before execution.</param>
        Task<IEnumerable<IDictionary<string, object>>> ExecuteSqlFromFileGeneric(string relativeFilePath, IDictionary<string, string> literalReplacements);

        /// <summary>
        /// Executes SQL contained in a KInspector script file and returns the results.
        /// </summary>
        /// <param name="relativeFilePath">The relative path of the .sql file to execute.</param>
        /// <param name="literalReplacements">Text within the .sql file matching the keys of this dictionary will be replaced by the values before execution.</param>
        /// <param name="parameters">Values which will be provided as parameters in the SQL query.</param>
        Task<IEnumerable<IDictionary<string, object>>> ExecuteSqlFromFileGeneric(string relativeFilePath, IDictionary<string, string> literalReplacements, dynamic parameters);

        /// <summary>
        /// Executes SQL contained in a KInspector script file and returns the results.
        /// </summary>
        /// <param name="relativeFilePath">The relative path of the .sql file to execute.</param>
        Task<T> ExecuteSqlFromFileScalar<T>(string relativeFilePath);

        /// <summary>
        /// Executes SQL contained in a KInspector script file and returns the results.
        /// </summary>
        /// <param name="relativeFilePath">The relative path of the .sql file to execute.</param>
        /// <param name="parameters">Values which will be provided as parameters in the SQL query.</param>
        Task<T> ExecuteSqlFromFileScalar<T>(string relativeFilePath, dynamic parameters);

        /// <summary>
        /// Executes SQL contained in a KInspector script file and returns the results.
        /// </summary>
        /// <param name="relativeFilePath">The relative path of the .sql file to execute.</param>
        /// <param name="literalReplacements">Text within the .sql file matching the keys of this dictionary will be replaced by the values before execution.</param>
        Task<T> ExecuteSqlFromFileScalar<T>(string relativeFilePath, IDictionary<string, string> literalReplacements);

        /// <summary>
        /// Executes SQL contained in a KInspector script file and returns the results.
        /// </summary>
        /// <param name="relativeFilePath">The relative path of the .sql file to execute.</param>
        /// <param name="literalReplacements">Text within the .sql file matching the keys of this dictionary will be replaced by the values before execution.</param>
        /// <param name="parameters">Values which will be provided as parameters in the SQL query.</param>
        Task<T> ExecuteSqlFromFileScalar<T>(string relativeFilePath, IDictionary<string, string> literalReplacements, dynamic parameters);
    }
}