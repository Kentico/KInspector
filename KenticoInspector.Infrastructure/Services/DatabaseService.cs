using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Dapper;

using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;

namespace KenticoInspector.Infrastructure.Services
{
    public class DatabaseService : IDatabaseService
    {
        private IDbConnection _connection;

        private IDbConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    throw new Exception($"You must run '{nameof(Configure)}' first.");
                }

                return _connection;
            }
        }

        public void Configure(DatabaseSettings databaseSettings)
        {
            _connection = DatabaseHelper.GetSqlConnection(databaseSettings);
        }

        public IEnumerable<T> ExecuteSqlFromFile<T>(string relativeFilePath)
        {
            //TODO: Catch exceptions
            return ExecuteSqlFromFile<T>(relativeFilePath, null, null);
        }

        public IEnumerable<T> ExecuteSqlFromFile<T>(string relativeFilePath, dynamic parameters)
        {
            return ExecuteSqlFromFile<T>(relativeFilePath, null, parameters);
        }

        public IEnumerable<T> ExecuteSqlFromFile<T>(string relativeFilePath, IDictionary<string, string> literalReplacements)
        {
            return ExecuteSqlFromFile<T>(relativeFilePath, literalReplacements, null);
        }

        public IEnumerable<T> ExecuteSqlFromFile<T>(string relativeFilePath, IDictionary<string, string> literalReplacements, dynamic parameters)
        {
            var query = FileHelper.GetSqlQueryText(relativeFilePath, literalReplacements);
            return Connection.Query<T>(query, (object)parameters);
        }

        public DataTable ExecuteSqlFromFileAsDataTable(string relativeFilePath)
        {
            var query = FileHelper.GetSqlQueryText(relativeFilePath);
            var result = new DataTable();
            result.Load(Connection.ExecuteReader(query));
            return result;
        }

        public IEnumerable<IDictionary<string, object>> ExecuteSqlFromFileGeneric(string relativeFilePath)
        {
            return ExecuteSqlFromFileGeneric(relativeFilePath, null, null);
        }

        public IEnumerable<IDictionary<string, object>> ExecuteSqlFromFileGeneric(string relativeFilePath, dynamic parameters)
        {
            return ExecuteSqlFromFileGeneric(relativeFilePath, null, parameters);
        }

        public IEnumerable<IDictionary<string, object>> ExecuteSqlFromFileGeneric(string relativeFilePath, IDictionary<string, string> literalReplacements)
        {
            return ExecuteSqlFromFileGeneric(relativeFilePath, literalReplacements, null);
        }

        public IEnumerable<IDictionary<string, object>> ExecuteSqlFromFileGeneric(string relativeFilePath, IDictionary<string, string> literalReplacements, dynamic parameters)
        {
            var query = FileHelper.GetSqlQueryText(relativeFilePath, literalReplacements);
            return Connection.Query(query, (object)parameters)
                .Select(x => (IDictionary<string, object>)x);
        }

        public T ExecuteSqlFromFileScalar<T>(string relativeFilePath)
        {
            return ExecuteSqlFromFileScalar<T>(relativeFilePath, null, null);
        }

        public T ExecuteSqlFromFileScalar<T>(string relativeFilePath, dynamic parameters)
        {
            return ExecuteSqlFromFileScalar<T>(relativeFilePath, null, parameters);
        }

        public T ExecuteSqlFromFileScalar<T>(string relativeFilePath, IDictionary<string, string> literalReplacements)
        {
            return ExecuteSqlFromFileScalar<T>(relativeFilePath, literalReplacements, null);
        }

        public T ExecuteSqlFromFileScalar<T>(string relativeFilePath, IDictionary<string, string> literalReplacements, dynamic parameters)
        {
            var query = FileHelper.GetSqlQueryText(relativeFilePath, literalReplacements);
            return Connection.QueryFirst<T>(query, (object)parameters);
        }
    }
}