using KenticoInspector.Actions.GlobalAdminSummary.Models;
using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;

using System;
using System.Collections.Generic;

namespace KenticoInspector.Actions.GlobalAdminSummary
{
    public class Action : AbstractAction<Terms,Options>
    {
        private readonly IDatabaseService databaseService;

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12", "13");

        public override IList<string> Tags => new List<string> {
            ActionTags.Reset,
            ActionTags.User
        };

        public Action(IDatabaseService databaseService, IModuleMetadataService moduleMetadataService) : base(moduleMetadataService)
        {
            this.databaseService = databaseService;
        }

        public override ActionResults Execute(Options options)
        {
            // No user provided, list users
            if (options.UserId == 0)
            {
                return GetListingResult();
            }

            if (options.UserId < 0)
            {
                return GetInvalidOptionsResult();
            }

            // Reset provided user
            databaseService.ExecuteSqlFromFileGeneric(Scripts.ResetAndEnableUser, new { UserID = options.UserId });
            var result = GetListingResult();
            result.Summary = Metadata.Terms.UserReset.With(new {
                userId = options.UserId
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
    }
}
