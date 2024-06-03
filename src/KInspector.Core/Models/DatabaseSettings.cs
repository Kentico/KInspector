namespace KInspector.Core.Models
{
    /// <summary>
    /// Represents the configuration required to connect to an instance's database.
    /// </summary>
    public class DatabaseSettings
    {
        /// <summary>
        /// The name of the database.
        /// </summary>
        public string? Database { get; set; }

        /// <summary>
        /// <c>True</c> if the database connection uses Integrated Security.
        /// </summary>
        public bool IntegratedSecurity { get; set; }

        /// <summary>
        /// The password used in the database credentials, if <see cref="IntegratedSecurity"/> is <c>false</c>.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// The name of the SQL Server.
        /// </summary>
        public string? Server { get; set; }

        /// <summary>
        /// The user name used in the database credentials, if <see cref="IntegratedSecurity"/> is <c>false</c>.
        /// </summary>
        public string? User { get; set; }

        /// <summary>
        /// If not <c>null</c>, this connection string is used to connect to the database and other properties are ignored.
        /// </summary>
        public string? AdministrationConnectionString { get; set; }
    }
}