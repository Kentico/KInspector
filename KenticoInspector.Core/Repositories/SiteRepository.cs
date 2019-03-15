using Dapper;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Repositories.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KenticoInspector.Core.Repositories
{
    public class SiteRepository : ISiteRepository
    {
        public Site GetSite(Instance instance, int siteID)
        {
            throw new NotImplementedException();
        }

        public List<Site> GetSites(Instance instance)
        {
            try
            {
                var instanceConnection = DatabaseHelper.GetSqlConnection(instance.DatabaseSettings);

                using (var connection = instanceConnection)
                {
                    var query = "SELECT SiteId as Id, SiteName as Name, SiteGUID as Guid, SiteDomainName as DomainName, SitePresentationURL as PresentationUrl, SiteIsContentOnly as ContentOnly from CMS_Site";
                    connection.Open();
                    var sites = connection.Query<Site>(query).ToList();
                    return sites;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
