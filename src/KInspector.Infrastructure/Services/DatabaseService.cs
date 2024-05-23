using System.Data;

using Dapper;

using KInspector.Core.Helpers;
using KInspector.Core.Models;
using KInspector.Core.Services.Interfaces;

namespace KInspector.Infrastructure.Services
{
    public class DatabaseService : IDatabaseService
    {
        private IDbConnection? _connection;

        public void Configure(DatabaseSettings databaseSettings)
        {
            _connection = DatabaseHelper.GetSqlConnection(databaseSettings);
        }

        public Task ExecuteNonQuery(string relativeFilePath, dynamic parameters)
        {
            var query = FileHelper.GetSqlQueryText(relativeFilePath);
            if (parameters is null)
            {
                return _connection.ExecuteAsync(query);
            }
            else
            {
                return _connection.ExecuteAsync(query, (object)parameters);
            }
        }

        public Task<IEnumerable<T>> ExecuteSqlFromFile<T>(string relativeFilePath)
        {
            return ExecuteSqlFromFile<T>(relativeFilePath, null, null);
        }

        public Task<IEnumerable<T>> ExecuteSqlFromFile<T>(string relativeFilePath, dynamic parameters)
        {
            return ExecuteSqlFromFile<T>(relativeFilePath, null, parameters);
        }

        public Task<IEnumerable<T>> ExecuteSqlFromFile<T>(string relativeFilePath, IDictionary<string, string> literalReplacements)
        {
            return ExecuteSqlFromFile<T>(relativeFilePath, literalReplacements, null);
        }

        public Task<IEnumerable<T>> ExecuteSqlFromFile<T>(string relativeFilePath, IDictionary<string, string>? literalReplacements, dynamic? parameters)
        {
            var query = FileHelper.GetSqlQueryText(relativeFilePath, literalReplacements);
            if (parameters is null)
            {
                return _connection.QueryAsync<T>(query);
            }
            else
            {
                return _connection.QueryAsync<T>(query, (object)parameters);
            }
        }

        public async Task<DataTable> ExecuteSqlFromFileAsDataTable(string relativeFilePath)
        {
            var query = FileHelper.GetSqlQueryText(relativeFilePath);
            var result = new DataTable();
            var data = await _connection.ExecuteReaderAsync(query);
            result.Load(data);
            
            return result;
        }

        public Task<IEnumerable<IDictionary<string, object>>> ExecuteSqlFromFileGeneric(string relativeFilePath)
        {
            return ExecuteSqlFromFileGeneric(relativeFilePath, null, null);
        }

        public Task<IEnumerable<IDictionary<string, object>>> ExecuteSqlFromFileGeneric(string relativeFilePath, dynamic parameters)
        {
            return ExecuteSqlFromFileGeneric(relativeFilePath, null, parameters);
        }

        public Task<IEnumerable<IDictionary<string, object>>> ExecuteSqlFromFileGeneric(string relativeFilePath, IDictionary<string, string> literalReplacements)
        {
            return ExecuteSqlFromFileGeneric(relativeFilePath, literalReplacements, null);
        }

        public async Task<IEnumerable<IDictionary<string, object>>> ExecuteSqlFromFileGeneric(string relativeFilePath, IDictionary<string, string>? literalReplacements, dynamic? parameters)
        {
            var query = FileHelper.GetSqlQueryText(relativeFilePath, literalReplacements);
            IEnumerable<dynamic> results;
            if (parameters is null)
            {
                results = await _connection.QueryAsync(query);
            }
            else
            {
                results = await _connection.QueryAsync(query, (object)parameters);
            }


            return results.Select(x => (IDictionary<string, object>)x);
        }

        public Task<T> ExecuteSqlFromFileScalar<T>(string relativeFilePath)
        {
            return ExecuteSqlFromFileScalar<T>(relativeFilePath, null, null);
        }

        public Task<T> ExecuteSqlFromFileScalar<T>(string relativeFilePath, dynamic parameters)
        {
            return ExecuteSqlFromFileScalar<T>(relativeFilePath, null, parameters);
        }

        public Task<T> ExecuteSqlFromFileScalar<T>(string relativeFilePath, IDictionary<string, string> literalReplacements)
        {
            return ExecuteSqlFromFileScalar<T>(relativeFilePath, literalReplacements, null);
        }

        public Task<T> ExecuteSqlFromFileScalar<T>(string relativeFilePath, IDictionary<string, string>? literalReplacements, dynamic? parameters)
        {
            var query = FileHelper.GetSqlQueryText(relativeFilePath, literalReplacements);
            if (parameters is null)
            {
                return _connection.QueryFirstAsync<T>(query);
            }
            else
            {
                return _connection.QueryFirstAsync<T>(query, (object)parameters);
            }
        }
    }
}