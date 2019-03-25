using Dapper;
using KenticoInspector.Infrastructure.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Repositories.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Dapper.FluentMap;
using KenticoInspector.Infrastructure.Conventions;

namespace KenticoInspector.Infrastructure.Repositories
{
    public class SiteRepository : ISiteRepository
    {
        public Site GetSite(Instance instance, int siteID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Site> GetSites(Instance instance)
        {
            try
            {
                FluentMapper.Initialize(config => {
                    config.AddConvention<SitePrefixConvention>()
                        .ForEntity<Site>();
                });

                var query = "SELECT SiteId, SiteName, SiteGUID, SiteDomainName, SitePresentationURL, SiteIsContentOnly from CMS_Site";
                var connection = DatabaseHelper.GetSqlConnection(instance.DatabaseSettings);
                var sites = connection.Query<Site>(query);
                return sites;
            }
            catch
            {
                return null;
            }
        }
    }
}
