using Dapper;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
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

        public IEnumerable<T> ExecuteSqlFromFile<T>(string fileName)
        {
            var query = FileHelper.GetSqlQueryText(fileName);
            return Connection.Query<T>(query);
        }
    }
}
