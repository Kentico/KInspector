using System;
using System.Data.SqlClient;
using System.IO;

namespace KInspector.Core
{
    public class InstanceInfo
    {
        public Uri Url { get; private set; }
        public DirectoryInfo Directory { get; private set; }
        public Version Version { get; private set; }

        public InstanceInfo(Version version, Uri url, DirectoryInfo directory)
        {
            if (version == null)
            {
                throw new ArgumentNullException("version");
            }

            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            if (directory == null)
            {
                throw new ArgumentNullException("directory");
            }

            Version = version;
            Url = url;
            Directory = directory;
        }
    }

    [Obsolete]
    public class KenticoInstanceConfig
    {
        public string Database { get; set; }
        public string Server { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Url { get; set; }
        public string Path { get; set; }

        /// <summary>
        /// Creates a connection string from the <see cref="KenticoInstanceConfig"/>.
        /// </summary>
        public string GetConnectionString()
        {
            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder();

            sb.UserID = User;
            sb.Password = Password;
            sb["Server"] = Server;
            sb["Database"] = Database;

            return sb.ConnectionString;
        }
    }
}
