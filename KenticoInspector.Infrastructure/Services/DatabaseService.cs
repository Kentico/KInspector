using Dapper;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace KenticoInspector.Infrastructure.Services
{
    class DatabaseService : IDatabaseService
    {
        private IDbConnection _connection;

        private IDbConnection Connection {
            get
            {
                if (_connection == null)
                {
                    throw new Exception("You must run 'ConfigureForInstance' first.");
                }

                return _connection;
            }
        }

        public void ConfigureForInstance(Instance instance)
        {
            _connection = DatabaseHelper.GetSqlConnection(instance);
        }

        public IEnumerable<T> ExecuteSqlFromFile<T>(string relativeFilePath, dynamic parameters = null)
        {
            var query = FileHelper.GetSqlQueryText(relativeFilePath);
            return Connection.Query<T>(query, (object)parameters);
        }

        public IEnumerable<IDictionary<string, object>> ExecuteSqlFromFileWithReplacements(string relativeFilePath, IDictionary<string, string> replacements, dynamic parameters = null)
        {
            var query = FileHelper.GetSqlQueryText(relativeFilePath);
            foreach (var replacement in replacements)
            {
                query = query.Replace(replacement.Key, replacement.Value);
            }
            
            return Connection.Query(query, (object)parameters).Select(x=>(IDictionary<string,object>)x);
        }

        public DataTable ExecuteSqlFromFileAsDataTable(string relativeFilePath)
        {
            var query = FileHelper.GetSqlQueryText(relativeFilePath);
            var result = new DataTable();
            result.Load(Connection.ExecuteReader(query));
            return result;
        }
    }
}
