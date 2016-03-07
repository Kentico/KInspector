using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace Kentico.KInspector.Core
{
    /// <summary>
    /// Basic service for communication with database.
    /// </summary>
    public class DatabaseService : IDatabaseService
    {
        private readonly string mConnectionString;
        private const int SQL_COMMAND_TIMEOUT_SECONDS = 90;


        public DatabaseService(InstanceConfig config)
        {
            mConnectionString = BuildConnectionString(config);
        }


        /// <summary>
        /// Builds connection string based on instance configuration.
        /// </summary>
        private string BuildConnectionString(InstanceConfig config)
        {
            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder();

            if (config.IntegratedSecurity)
            {
                sb.IntegratedSecurity = true;
            }
            else
            {
                sb.UserID = config.User;
                sb.Password = config.Password;
            }

            sb["Server"] = config.Server;
            sb["Database"] = config.Database;

            return sb.ConnectionString;
        }


        /// <summary>
        /// Executes the query in <paramref name="sql"/> and returns the result object. 
        /// </summary>
        /// <remarks>
        /// You can put the result directly into <see cref="ModuleResults.Result"/>.
        /// </remarks>
        public T ExecuteAndGetScalar<T>(string sql) where T : IConvertible
        {
            using (var connection = new SqlConnection(mConnectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = sql;
                command.CommandTimeout = SQL_COMMAND_TIMEOUT_SECONDS;
                connection.Open();
                return (T)Convert.ChangeType(command.ExecuteScalar(), typeof(T));
            }
        }


        /// <summary>
        /// Executes the query in <paramref name="sql"/> and returns the result table. 
        /// </summary>
        /// <remarks>
        /// You can put the result directly into <see cref="ModuleResults.Result"/>.
        /// </remarks>
        private DataTable ExecuteAndGetTable(string sql, params SqlParameter[] parameters)
        {
            DataSet result = ExecuteAndGetDataSet(sql, parameters);
            if (result.Tables.Count > 0)
            {
                return result.Tables[0];
            }
            else
            {
                return new DataTable();
            }
        }


        /// <summary>
        /// Executes a SQL file in <paramref name="filePath"/> and returns the whole table. 
        /// The file must be in './Scripts/' folder.
        /// </summary>
        /// <remarks>
        /// You can put the result directly into <see cref="ModuleResults.Result"/>.
        /// </remarks>
        /// <param name="filePath">Path of the file in './Scripts/' folder</param>
        /// <param name="parameters">Optional parameters send to SQL script</param>
        public DataTable ExecuteAndGetTableFromFile(string filePath, params SqlParameter[] parameters)
        {
            using (var sr = new StreamReader("./Scripts/" + filePath))
            {
                var fileContents = sr.ReadToEnd();
                return ExecuteAndGetTable(fileContents, parameters);
            }
        }


        /// <summary>
        /// Executes the query in <paramref name="sql"/> and returns the result data set. 
        /// </summary>
        /// <remarks>
        /// You can put the result directly into <see cref="ModuleResults.Result"/>.
        /// </remarks>
        public DataSet ExecuteAndGetDataSet(string sql, params SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(mConnectionString))
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter();
                SqlCommand command = connection.CreateCommand();
                command.CommandText = sql;
                command.CommandTimeout = SQL_COMMAND_TIMEOUT_SECONDS;
                command.Parameters.AddRange(parameters);
                dataAdapter.SelectCommand = command;

                DataSet dataSet = new DataSet();

                connection.Open();
                dataAdapter.Fill(dataSet);

                if (dataSet.Tables.Count >= 2)
                {
                    // Check for dummy tables containing the name the table that follows 
                    // (dummy table has exactly one column named "#KenticoNextTableName")
                    for (int i = dataSet.Tables.Count - 2; i >= 0; i--)
                    {
                        DataTable table = dataSet.Tables[i];
                        if (table.Columns.Count == 1 && table.Columns[0].ColumnName.Equals("#KInspectorNextTableName", StringComparison.InvariantCultureIgnoreCase))
                        {
                            // This is dummy table - set the name of the following table
                            dataSet.Tables[i + 1].TableName = table.Rows[0][0].ToString();

                            // Remove dummy table
                            dataSet.Tables.Remove(table);

                            // We have name for table [i+1], table [i] is dummy and is removed, so 
                            // table [i-1] cannot contain the name of another table and can be safely skipped.
                            i--;
                        }
                    }
                }

                return dataSet;
            }
        }


        /// <summary>
        /// Executes a SQL file in <paramref name="filePath"/> and returns the whole data set. 
        /// The file must be in './Scripts/' folder.
        /// </summary>
        /// <remarks>
        /// You can put the result directly into <see cref="ModuleResults.Result"/>.
        /// </remarks>
        /// <param name="filePath">Path of the file in './Scripts/' folder</param>
        public DataSet ExecuteAndGetDataSetFromFile(string filePath)
        {
            using (var sr = new StreamReader("./Scripts/" + filePath))
            {
                var fileContents = sr.ReadToEnd();
                return ExecuteAndGetDataSet(fileContents);
            }
        }


        /// <summary>
        /// Executes the query and returns all the PRINT statements that are included.
        /// </summary>
        /// <remarks>
        /// You can put the result directly into <see cref="ModuleResults.Result"/>.
        /// </remarks>
        private List<string> ExecuteAndGetPrints(string sql)
        {
            var output = new List<string>();

            using (SqlConnection conn = new SqlConnection(mConnectionString))
            {
                conn.Open();

                // This is the way how to handle PRINT statements
                conn.InfoMessage += (s, ea) =>
                {
                    // All the PRINT statements are saved separately in ea.Errors collection
	                output.AddRange(ea.Errors.Cast<SqlError>().Select(error => error.Message));
                };

                var command = new SqlCommand(sql, conn)
                {
                    CommandTimeout = SQL_COMMAND_TIMEOUT_SECONDS
                };
                command.ExecuteNonQuery();
            }

            return output;
        }


        /// <summary>
        /// Similar to <see cref="ExecuteAndGetPrints"/>, but takes SQL script URL as a parameter.
        /// Reads the input from './Scripts/' folder, the SQL script must therefore be there.
        /// </summary>
        /// <remarks>
        /// You can put the result directly into <see cref="ModuleResults.Result"/>.
        /// </remarks>
        public List<string> ExecuteAndGetPrintsFromFile(string filePath)
        {
            using (var sr = new StreamReader("./Scripts/" + filePath))
            {
                var fileContents = sr.ReadToEnd();
                return ExecuteAndGetPrints(fileContents);
            }
        }


        /// <summary>
        /// Returns setting value for a certain site. 
        /// If setting is not set for this site, then the global value is received.
        /// </summary>
        public T GetSetting<T>(string key, string siteName = "") where T : IConvertible
        {
            return ExecuteAndGetScalar<T>(
                string.Format(@"SELECT ISNULL(
                                (SELECT KeyValue 
                                FROM CMS_SettingsKey AS SK LEFT JOIN CMS_Site AS S ON S.SiteID = SK.SiteID 
                                WHERE S.SiteName = '{0}' AND KeyName = '{1}'),
                                (SELECT KeyValue FROM CMS_SettingsKey AS SK LEFT JOIN CMS_Site AS S ON S.SiteID = SK.SiteID 
                                    WHERE S.SiteName IS NULL AND KeyName = '{1}'))", siteName, key));
        }
    }
}
