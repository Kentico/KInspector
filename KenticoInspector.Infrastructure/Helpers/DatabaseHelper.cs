using KenticoInspector.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace KenticoInspector.Infrastructure.Helpers
{
    public class DatabaseHelper
    {
        public static string GetConnectionString(DatabaseSettings databaseSettings)
        {
            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder();

            if (databaseSettings.IntegratedSecurity)
            {
                sb.IntegratedSecurity = true;
            }
            else
            {
                sb.UserID = databaseSettings.User;
                sb.Password = databaseSettings.Password;
            }

            sb["Server"] = databaseSettings.Server;
            sb["Database"] = databaseSettings.Database;

            return sb.ConnectionString;
        }

        public static IDbConnection GetSqlConnection(DatabaseSettings databaseSettings)
        {
            var connectionString = GetConnectionString(databaseSettings);
            return new SqlConnection(connectionString);
        }
    }
}
