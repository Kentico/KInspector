using System;
using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models.Results;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.UserPasswordAnalysis.Models;
using KenticoInspector.Reports.UserPasswordAnalysis.Models.Data;
using KenticoInspector.Reports.UserPasswordAnalysis.Models.Data.Results;

namespace KenticoInspector.Reports.UserPasswordAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;

        public Report(IDatabaseService databaseService, IReportMetadataService reportMetadataService)
            : base(reportMetadataService)
        {
            this.databaseService = databaseService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12");

        public override IList<string> Tags => new List<string>
        {
            ReportTags.Security,
            ReportTags.Configuration
        };

        public static IEnumerable<string> ExcludedUserNames => new List<string>
        {
            "public"
        };

        public override ReportResults GetResults()
        {
            var users = databaseService.ExecuteSqlFromFile<CmsUser>(
                Scripts.GetEnabledAndNotExternalUsers,
                new { ExcludedUserNames }
                );

            var usersWithEmptyPasswords = GetUsersWithEmptyPasswords(users);

            var usersWithPlaintextPasswords = GetUsersWithPlaintextPasswords(users);

            return CompileResults(usersWithEmptyPasswords, usersWithPlaintextPasswords);
        }

        private static IEnumerable<CmsUserResultWithPasswordFormat> GetUsersWithEmptyPasswords(
            IEnumerable<CmsUser> users)
        {
            return users
                .Where(user => string.IsNullOrEmpty(user.UserPassword))
                .Select(user => new CmsUserResultWithPasswordFormat(user));
        }

        private static IEnumerable<CmsUserResult> GetUsersWithPlaintextPasswords(IEnumerable<CmsUser> users)
        {
            return users
                .Where(user => string.IsNullOrEmpty(user.UserPasswordFormat))
                .Select(user => new CmsUserResult(user));
        }

        private ReportResults CompileResults(
            IEnumerable<CmsUserResultWithPasswordFormat> usersWithEmptyPasswords,
            IEnumerable<CmsUserResult> usersWithPlaintextPasswords)
        {
            if (!usersWithEmptyPasswords.Any() && !usersWithPlaintextPasswords.Any())
            {
                return new ReportResults
                {
                    Status = ReportResultsStatus.Good,
                    Summary = Metadata.Terms.GoodSummary
                };
            }

            var errorReportResults = new ReportResults
            {
                Status = ReportResultsStatus.Error
            };

            var emptyCount = usersWithEmptyPasswords.Count();

            errorReportResults.Data.Add(
                usersWithEmptyPasswords.AsResult().WithLabel(Metadata.Terms.TableLabels.EmptyPasswords)
                );

            var plaintextCount = usersWithPlaintextPasswords.Count();

            errorReportResults.Data.Add(
                usersWithPlaintextPasswords.AsResult().WithLabel(Metadata.Terms.TableLabels.PlaintextPasswords)
                );

            errorReportResults.Summary = Metadata.Terms.ErrorSummary.With(new { emptyCount, plaintextCount });

            return errorReportResults;
        }
    }
}