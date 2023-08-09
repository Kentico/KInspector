using KenticoInspector.Actions.SiteStatusSummary.Models;
using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Actions.SiteStatusSummary
{
    public class Action : AbstractAction<Terms,Options>
    {
        private readonly IDatabaseService databaseService;

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("12", "13");

        public override IList<string> Tags => new List<string> {
            ModuleTags.Site,
            ModuleTags.Configuration
        };

        public Action(IDatabaseService databaseService, IModuleMetadataService moduleMetadataService) : base(moduleMetadataService)
        {
            this.databaseService = databaseService;
        }

        public override ActionResults Execute(Options options)
        {
            // No site provided, list sites
            if (options.SiteId == null)
            {
                return GetListingResult();
            }

            // Validate options
            var sites = databaseService.ExecuteSqlFromFile<CmsSite>(Scripts.GetSiteSummary);
            if (options.SiteId <= 0 ||
                !sites.Select(s => s?.ID).Contains(options.SiteId))
            {
                return GetInvalidOptionsResult();
            }

            // Stop provided site
            databaseService.ExecuteSqlFromFileGeneric(Scripts.StopSite, new { SiteID = options.SiteId });
            var result = GetListingResult();
            result.Summary = Metadata.Terms.SiteStopped.With(new
            {
                siteId = options.SiteId
            });

            return result;
        }

        public override ActionResults GetInvalidOptionsResult()
        {
            var result = GetListingResult();
            result.Status = ResultsStatus.Error;
            result.Summary = Metadata.Terms.InvalidOptions;

            return result;
        }

        private ActionResults GetListingResult()
        {
            var sites = databaseService.ExecuteSqlFromFile<CmsSite>(Scripts.GetSiteSummary);
            var data = new TableResult<CmsSite>()
            {
                Name = Metadata.Terms.TableTitle,
                Rows = sites
            };

            return new ActionResults
            {
                Type = ResultsType.Table,
                Status = ResultsStatus.Information,
                Summary = Metadata.Terms.ListSummary,
                Data = data
            };
        }
    }
}
