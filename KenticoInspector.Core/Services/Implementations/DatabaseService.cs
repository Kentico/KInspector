using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using System;
using System.Data;
using System.Data.SqlClient;

namespace KenticoInspector.Core.Services.Implementations
{
    public class DatabaseService : IDatabaseService
    {
        private readonly string _connectionString;
        private readonly int _sqlCommandTimeout = 90;

        public DatabaseService(InstanceConfiguration instanceConfiguration)
        {
            if (instanceConfiguration == null)
            {
                throw new ArgumentNullException("instanceConfiguration");
            }

            _connectionString = BuildConnectionString(instanceConfiguration);
            _sqlCommandTimeout = instanceConfiguration.DatabaseConfiguration?.CommandTimeout ?? _sqlCommandTimeout;
        }

        public DataSet ExecuteAndGetDataSet(string sql, params IDbDataParameter[] parameters)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = CreateSqlCommand(connection, sql);

                connection.Open();

                SqlDataAdapter dataAdapter = new SqlDataAdapter();
                dataAdapter.SelectCommand = command;

                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet);

                return dataSet;
            }
        }

        public DataSet ExecuteAndGetDataSetFromFile(string relativeFilePath, params IDbDataParameter[] parameters)
        {
            var sql = FileHelper.GetSqlScriptText(relativeFilePath);
            return ExecuteAndGetDataSet(sql, parameters);
        }

        public DataTable ExecuteAndGetDataTable(string sql, params IDbDataParameter[] parameters)
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

        public DataTable ExecuteAndGetDataTableFromFile(string relativeFilePath, params IDbDataParameter[] parameters)
        {
            var sql = FileHelper.GetSqlScriptText(relativeFilePath);
            return ExecuteAndGetDataTable(sql, parameters);
        }

        public T ExecuteAndGetScalar<T>(string sql) where T : IConvertible
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = CreateSqlCommand(connection, sql);

                connection.Open();
                return (T)Convert.ChangeType(command.ExecuteScalar(), typeof(T));
            }
        }

        private string BuildConnectionString(InstanceConfiguration instanceConfiguration)
        {
            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder();

            if (instanceConfiguration.DatabaseConfiguration.IntegratedSecurity)
            {
                sb.IntegratedSecurity = true;
            }
            else
            {
                sb.UserID = instanceConfiguration.DatabaseConfiguration.User;
                sb.Password = instanceConfiguration.DatabaseConfiguration.Password;
            }

            sb["Server"] = instanceConfiguration.DatabaseConfiguration.ServerName;
            sb["Database"] = instanceConfiguration.DatabaseConfiguration.DatabaseName;

            return sb.ConnectionString;
        }

        private SqlCommand CreateSqlCommand(SqlConnection sqlConnection, string sql, params IDbDataParameter[] parameters)
        {
            var command = sqlConnection.CreateCommand();
            command.CommandText = sql;
            command.CommandTimeout = _sqlCommandTimeout;
            command.Parameters.AddRange(parameters);

            return command;
        }
    }
}