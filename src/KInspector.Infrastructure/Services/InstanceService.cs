using Dapper;

using KInspector.Core.Helpers;
using KInspector.Core.Models;
using KInspector.Core.Services.Interfaces;

namespace KInspector.Infrastructure.Services
{
    public class InstanceService : IInstanceService
    {
        private readonly IConfigService _configService;
        private readonly IVersionService _versionService;

        public InstanceService(
            IConfigService configService,
            IVersionService versionService)
        {
            _configService = configService;
            _versionService = versionService;
        }

        public InstanceDetails GetInstanceDetails(Guid instanceGuid)
        {
            var instance = _configService.GetInstance(instanceGuid) ?? throw new InvalidOperationException($"No instance with GUID '{instanceGuid}.'");

            return GetInstanceDetails(instance);
        }

        public InstanceDetails GetInstanceDetails(Instance? instance)
        {
            ArgumentNullException.ThrowIfNull(instance);
            var instanceDetails = new InstanceDetails
            {
                AdministrationVersion = _versionService.GetKenticoAdministrationVersion(instance),
                AdministrationDatabaseVersion = _versionService.GetKenticoDatabaseVersion(instance.DatabaseSettings),
                Sites = GetSites(instance.DatabaseSettings)
            };

            return instanceDetails;
        }

        private static IList<Site> GetSites(DatabaseSettings databaseSettings)
        {
            try
            {
                var query = @"
                    SELECT
                        SiteId as Id,
                        SiteName as Name,
                        SiteGUID as Guid,
                        SiteDomainName as DomainName,
                        SitePresentationURL as PresentationUrl,
                        SiteStatus as Status
                    FROM CMS_Site";

                var connection = DatabaseHelper.GetSqlConnection(databaseSettings);
                var sites = connection.Query<Site>(query).ToList();

                return sites;
            }
            catch
            {
                return Array.Empty<Site>();
            }
        }
    }
}