using System;
using System.IO;

namespace Kentico.KInspector.Core
{
    public class InstanceInfo : IInstanceInfo
    {
        private Lazy<DatabaseService> dbService;
        private Lazy<Version> version;
        private Lazy<Uri> uri;
        private Lazy<DirectoryInfo> directory;


        /// <summary>
        /// URI of the application instance
        /// </summary>
        public Uri Uri => uri.Value;

        /// <summary>
        /// Directory of the application instance
        /// </summary>
        public DirectoryInfo Directory => directory.Value;

        /// <summary>
        /// Version of the instance based on the database setting key.
        /// </summary>
        public Version Version => version.Value;

        /// <summary>
        /// Configuration with instance information.
        /// </summary>
        public InstanceConfig Config { get; private set; }


        /// <summary>
        /// Database service to communicate with the instance database.
        /// </summary>
        public IDatabaseService DBService => dbService.Value;

        /// <summary>
        /// Creates instance information based on configuration.
        /// </summary>
        /// <param name="config">Instance configuration</param>
        public InstanceInfo(InstanceConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("version");
            }

            Config = config;

            dbService = new Lazy<DatabaseService>(() => new DatabaseService(Config));
            version = new Lazy<Version>(GetKenticoVersion);

            // Ensure backslash to the Config.Url to support VirtualPath URLs.
            // Sometimes the website is running under virtual path and 
            // the URL looks like this http://localhost/kentico8
            // Some modules (RobotsTxtModule, CacheItemsModle, ...) try 
            // to append the relative path to the base URL but
            // without trailing slash, the relative path is replaced.
            // E.g.: 
            //      var uri = new Uri("http://localhost/kentico8");
            //      new Uri(uri, "robots.txt"); -> http://localhost/robots.txt
            // 
            // With trailing slash, the relative path is appended as expected.
            //      var uri = new Uri("http://localhost/kentico8/");
            //      new Uri(uri, "robots.txt"); -> http://localhost/kentico8/robots.txt
            uri = new Lazy<Uri>(() => new Uri(Config.Url.EndsWith("/") ? Config.Url : Config.Url + "/"));
            directory = new Lazy<DirectoryInfo>(() => new DirectoryInfo(Config.Path));
        }


        /// <summary>
        /// Gets the version of Kentico.
        /// </summary>
        private Version GetKenticoVersion()
        {
            string version = DBService.ExecuteAndGetScalar<string>("SELECT KeyValue FROM CMS_SettingsKey WHERE KeyName = 'CMSDBVersion'");
            return new Version(version);
        }
    }
}
