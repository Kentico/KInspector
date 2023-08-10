using KenticoInspector.Actions.StagingServerSummary.Models;
using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Actions.StagingServerSummary
{
    public class Action : AbstractAction<Terms, Options>
    {
        private readonly IDatabaseService databaseService;

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("12", "13");

        public override IList<string> Tags => new List<string> {
            ModuleTags.Configuration,
            ModuleTags.Staging
        };

        public Action(IDatabaseService databaseService, IModuleMetadataService moduleMetadataService) : base(moduleMetadataService)
        {
            this.databaseService = databaseService;
        }

        public override ActionResults Execute(Options options)
        {
            databaseService.ExecuteSqlFromFileGeneric(Scripts.DisableServer, new { ServerID = options.ServerId });
            var result = ExecuteListing();
            result.Status = ResultsStatus.Good;
            result.Summary = Metadata.Terms.ServerDisabled.With(new
            {
                serverId = options.ServerId
            });

            return result;
        }

        public override ActionResults ExecutePartial(Options options)
        {
            // All options are required for this action
            throw new NotImplementedException();
        }

        public override ActionResults ExecuteListing()
        {
            var servers = databaseService.ExecuteSqlFromFile<StagingServer>(Scripts.GetStagingServerSummary);
            var data = new TableResult<StagingServer>()
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

        public override ActionResults GetInvalidOptionsResult()
        {
            var result = ExecuteListing();
            result.Status = ResultsStatus.Error;
            result.Summary = Metadata.Terms.InvalidOptions;

            return result;
        }

        public override bool ValidateOptions(Options options)
        {
            var servers = databaseService.ExecuteSqlFromFile<StagingServer>(Scripts.GetStagingServerSummary);

            return options.ServerId > 0 &&
                servers.Any(s => s.ID == options.ServerId) &&
                servers.FirstOrDefault(s => s.ID == options.ServerId).Enabled;
        }
    }
}