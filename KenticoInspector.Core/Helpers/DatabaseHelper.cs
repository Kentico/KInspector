using System.Data;
using System.Data.SqlClient;

using KenticoInspector.Core.Models;

namespace KenticoInspector.Core.Helpers
{
    public class DatabaseHelper
    {
        public static IDbConnection GetSqlConnection(DatabaseSettings databaseSettings)
        {
            var connectionString = GetConnectionString(databaseSettings);

            return GetSqlConnection(connectionString);
        }

        public static IDbConnection GetSqlConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        private static string GetConnectionString(DatabaseSettings databaseSettings)
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
    }
}