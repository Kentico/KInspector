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
            throw new NotImplementedException();
        }
    }
}
