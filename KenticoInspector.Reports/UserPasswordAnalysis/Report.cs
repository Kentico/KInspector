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

        public Report(IDatabaseService databaseService, IModuleMetadataService moduleMetadataService)
            : base(moduleMetadataService)
        {
            this.databaseService = databaseService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12", "13");

        public override IList<string> Tags => new List<string>
        {
            ModuleTags.Security,
            ModuleTags.Configuration
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
            IEnumerable<CmsUserResult> usersWithEmptyPasswords,
            IEnumerable<CmsUserResult> usersWithPlaintextPasswords)
        {
            if (!usersWithEmptyPasswords.Any() && !usersWithPlaintextPasswords.Any())
            {
                return new ReportResults
                {
                    Type = ResultsType.String,
                    Status = ResultsStatus.Good,
                    Summary = Metadata.Terms.GoodSummary
                };
            }

            var errorReportResults = new ReportResults
            {
                Type = ResultsType.TableList,
                Status = ResultsStatus.Error,
                Data = new List<TableResult<CmsUserResult>>()
            };

            var emptyCount = IfAnyAddTableResult(
                errorReportResults.Data,
                usersWithEmptyPasswords,
                Metadata.Terms.TableTitles.EmptyPasswords
                );

            var plaintextCount = IfAnyAddTableResult(
                errorReportResults.Data,
                usersWithPlaintextPasswords,
                Metadata.Terms.TableTitles.PlaintextPasswords
                );

            errorReportResults.Summary = Metadata.Terms.ErrorSummary.With(new { emptyCount, plaintextCount });

            return errorReportResults;
        }

        private static int IfAnyAddTableResult<T>(dynamic data, IEnumerable<T> results, Term tableNameTerm)
        {
            if (results.Any())
            {
                var tableResult = new TableResult<T>
                {
                    Name = tableNameTerm,
                    Rows = results
                };

                data.Add(tableResult);
            }

            return results.Count();
        }
    }
}