using KenticoInspector.Actions.GlobalAdminSummary.Models;
using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Actions.GlobalAdminSummary
{
    public class Action : AbstractAction<Terms, Options>
    {
        private readonly IDatabaseService databaseService;

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12", "13");

        public override IList<string> Tags => new List<string> {
            ModuleTags.Reset,
            ModuleTags.User
        };

        public Action(IDatabaseService databaseService, IModuleMetadataService moduleMetadataService) : base(moduleMetadataService)
        {
            this.databaseService = databaseService;
        }

        public override ActionResults Execute(Options options)
        {
            databaseService.ExecuteSqlFromFileGeneric(Scripts.ResetAndEnableUser, new { UserID = options.UserId });
            var result = ExecuteListing();
            result.Status = ResultsStatus.Good;
            result.Summary = Metadata.Terms.UserReset.With(new
            {
                userId = options.UserId
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
            var administratorUsers = databaseService.ExecuteSqlFromFile<CmsUser>(Scripts.GetAdministrators);
            var data = new TableResult<CmsUser>()
            {
                Name = Metadata.Terms.TableTitle,
                Rows = administratorUsers
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
            var administratorUsers = databaseService.ExecuteSqlFromFile<CmsUser>(Scripts.GetAdministrators);

            return options.UserId > 0 &&
                administratorUsers.Any(u => u.UserID == options.UserId) &&
                (
                    !administratorUsers.FirstOrDefault(u => u.UserID == options.UserId).Enabled ||
                    !String.IsNullOrEmpty(administratorUsers.FirstOrDefault(u => u.UserID == options.UserId).Password)
                );
        }
    }
}