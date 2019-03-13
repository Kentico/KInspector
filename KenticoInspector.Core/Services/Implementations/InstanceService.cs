using KenticoInspector.Core.Models;
using System;

namespace KenticoInspector.Core.Services.Implementations
{
    public class InstanceService : IInstanceService
    {
        private readonly IDatabaseService _databaseService;

        public InstanceService(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public Instance GetInstance(InstanceConfiguration instanceConfiguration)
        {
            if (instanceConfiguration == null)
            {
                throw new ArgumentNullException("instanceConfiguration");
            }

            var instance = new Instance();
            instance.InstanceConfiguration = instanceConfiguration;
            instance.KenticoDatabaseVersion = GetKenticoDatabaseVersion();
            instance.KenticoAdministrationVersion = GetKenticoAdministrationVersion();

            return instance;
        }

        private Version GetKenticoAdministrationVersion()
        {
            // TODO: Read version from administration files
            return new Version(0, 0);
        }

        private Version GetKenticoDatabaseVersion()
        {
            string version = _databaseService.ExecuteAndGetScalar<string>("SELECT KeyValue FROM CMS_SettingsKey WHERE KeyName = 'CMSDBVersion'");
            return new Version(version);
        }
    }
}
