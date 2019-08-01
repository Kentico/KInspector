using System;
using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.UserPasswordAnalysis.Models;
using KenticoInspector.Reports.UserPasswordAnalysis.Models.Data;
using KenticoInspector.Reports.UserPasswordAnalysis.Models.Data.Results;

namespace KenticoInspector.Reports.UserPasswordAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;

        public Report(IDatabaseService databaseService, IReportMetadataService reportMetadataService) : base(reportMetadataService)
        {
            this.databaseService = databaseService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12");

        public override IList<string> Tags => new List<string> {
            ReportTags.Security,
            ReportTags.Configuration
        };

        public static IEnumerable<string> ExcludedUserNames => new List<string> {
            "public"
        };

        public override ReportResults GetResults()
        {
            var cmsUsers = databaseService.ExecuteSqlFromFile<CmsUser>(Scripts.GetEnabledAndNotExternalUsers, new { ExcludedUserNames });

            var cmsUsersWithEmptyPasswords = GetCmsUsersWithEmptyPasswords(cmsUsers);

            var cmsUsersWithPlaintextPasswords = GetUsersWithPlaintextPasswords(cmsUsers);

            return CompileResults(cmsUsersWithEmptyPasswords, cmsUsersWithPlaintextPasswords);
        }

        private IEnumerable<CmsUserResult> GetCmsUsersWithEmptyPasswords(IEnumerable<CmsUser> cmsUsers)
        {
            return cmsUsers
                .Where(cmsUser => string.IsNullOrEmpty(cmsUser.UserPassword))
                .Select(cmsUser => new CmsUserResultWithPasswordFormat(cmsUser));
        }

        private IEnumerable<CmsUserResult> GetUsersWithPlaintextPasswords(IEnumerable<CmsUser> cmsUsers)
        {
            return cmsUsers
                .Where(cmsUser => string.IsNullOrEmpty(cmsUser.UserPasswordFormat))
                .Select(cmsUser => new CmsUserResult(cmsUser));
        }

        private ReportResults CompileResults(IEnumerable<CmsUserResult> cmsUsersWithEmptyPasswords, IEnumerable<CmsUserResult> cmsUsersWithPlaintextPasswords)
        {
            if (!cmsUsersWithEmptyPasswords.Any() && !cmsUsersWithPlaintextPasswords.Any())
            {
                return new ReportResults
                {
                    Type = ReportResultsType.String,
                    Status = ReportResultsStatus.Good,
                    Summary = Metadata.Terms.GoodSummary
                };
            }

            var errorReportResults = new ReportResults
            {
                Type = ReportResultsType.TableList,
                Status = ReportResultsStatus.Error,
                Data = new List<TableResult<CmsUserResult>>()
            };

            var (emptyCount, usersWithEmptyPasswordsResult) = GetTableResult(cmsUsersWithEmptyPasswords, Metadata.Terms.TableTitles.EmptyPasswords);

            if (emptyCount > 0) errorReportResults.Data.Add(usersWithEmptyPasswordsResult);

            var (plaintextCount, usersWithPlaintextPasswordsResult) = GetTableResult(cmsUsersWithPlaintextPasswords, Metadata.Terms.TableTitles.PlaintextPasswords);

            if (plaintextCount > 0) errorReportResults.Data.Add(usersWithPlaintextPasswordsResult);

            errorReportResults.Summary = Metadata.Terms.ErrorSummary.With(new { emptyCount, plaintextCount });

            return errorReportResults;
        }

        private (int, TableResult<CmsUserResult>) GetTableResult(IEnumerable<CmsUserResult> cmsUsers, Term term)
        {
            var count = cmsUsers.Count();

            var tableResult = new TableResult<CmsUserResult>
            {
                Name = term,
                Rows = cmsUsers
            };

            return (count, tableResult);
        }
    }
}