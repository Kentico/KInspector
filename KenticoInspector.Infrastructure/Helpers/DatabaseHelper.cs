using KenticoInspector.Core.Models;
using System.Data;
using System.Data.SqlClient;

namespace KenticoInspector.Infrastructure.Helpers
{
    public class DatabaseHelper
    {
        public static string GetConnectionString(Instance instance)
        {
            return GetConnectionString(instance.DatabaseSettings);
        }

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

        public static IDbConnection GetSqlConnection(Instance instance)
        {
            var connectionString = GetConnectionString(instance);
            return GetSqlConnection(connectionString);
        }

        public static IDbConnection GetSqlConnection(DatabaseSettings databaseSettings)
        {
            var connectionString = GetConnectionString(databaseSettings);
            return GetSqlConnection(connectionString);
        }

        public static IDbConnection GetSqlConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
    }
}