using KenticoInspector.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace KenticoInspector.Core.Helpers
{
    public class DatabaseHelper
    {
        public static string BuildConnectionString(DatabaseSettings databaseSettings)
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
            var connectionString = BuildConnectionString(databaseSettings);
            return new SqlConnection(connectionString);
        }
    }
}
