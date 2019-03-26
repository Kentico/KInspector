using Dapper;
using Dapper.FluentMap;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Repositories.Interfaces;
using KenticoInspector.Infrastructure.Conventions;
using KenticoInspector.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Infrastructure.Repositories
{
    public class SiteRepository : ISiteRepository
    {
        public Site GetSite(Instance instance, int siteID)
        {
            throw new NotImplementedException();
        }

        public IList<Site> GetSites(Instance instance)
        {
            try
            {
                FluentMapper.Initialize(config => {
                    config.AddConvention<SitePrefixConvention>()
                        .ForEntity<Site>();
                });

                var query = "SELECT SiteId, SiteName, SiteGUID, SiteDomainName, SitePresentationURL, SiteIsContentOnly from CMS_Site";
                var connection = DatabaseHelper.GetSqlConnection(instance.DatabaseSettings);
                var sites = connection.Query<Site>(query).ToList();
                return sites;
            }
            catch
            {
                return null;
            }
        }
    }
}
