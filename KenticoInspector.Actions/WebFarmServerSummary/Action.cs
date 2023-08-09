using KenticoInspector.Actions.WebFarmServerSummary.Models;
using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;

using System;
using System.Collections.Generic;

namespace KenticoInspector.Actions.WebFarmServerSummary
{
    public class Action : AbstractAction<Terms,Options>
    {
        private readonly IDatabaseService databaseService;

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("12", "13");

        public override IList<string> Tags => new List<string> {
            ActionTags.Configuration
        };

        public Action(IDatabaseService databaseService, IModuleMetadataService moduleMetadataService) : base(moduleMetadataService)
        {
            this.databaseService = databaseService;
        }

        public override ActionResults Execute(Options options)
        {
            // No server provided, list servers
            if (options.ServerID == 0)
            {
                return GetListingResult();
            }

            if (options.ServerID < 0)
            {
                return GetInvalidOptionsResult();
            }

            // Disable provided server
            databaseService.ExecuteSqlFromFileGeneric(Scripts.DisableServer, new { ServerID = options.ServerID });
            var result = GetListingResult();
            result.Summary = Metadata.Terms.ServerDisabled.With(new
            {
                serverId = options.ServerID
            });

            return result;
        }

        public override ActionResults GetInvalidOptionsResult()
        {
            return new ActionResults {
                Status = ResultsStatus.Error,
                Summary = Metadata.Terms.InvalidOptions
            };
        }

        private ActionResults GetListingResult()
        {
            var servers = databaseService.ExecuteSqlFromFile<WebFarmServer>(Scripts.GetWebFarmServerSummary);
            var data = new TableResult<WebFarmServer>()
            {
                Name = Metadata.Terms.TableTitle,
                Rows = servers
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
