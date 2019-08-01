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

        public IEnumerable<string> ExcludedUserNames => new List<string> {
            "public"
        };

        public override ReportResults GetResults()
        {
            var userDtos = databaseService.ExecuteSqlFromFile<UserDto>(Scripts.GetEnabledAndNotExternalUsers);

            var userDtosWithoutExcludedUsers = GetUsersWithoutExcludedUsers(userDtos);

            var userDtosWithEmptyPasswords = GetUsersWithEmptyPasswords(userDtosWithoutExcludedUsers);

            var userDtosWithPlaintextPasswords = GetUsersWithPlaintextPasswords(userDtosWithoutExcludedUsers);

            return CompileResults(userDtosWithEmptyPasswords, userDtosWithPlaintextPasswords);
        }

        private IEnumerable<UserDto> GetUsersWithoutExcludedUsers(IEnumerable<UserDto> userDtos)
        {
            return userDtos
                .Where(userDto => !ExcludedUserNames.Contains(userDto.UserName));
        }

        private IEnumerable<UserResult> GetUsersWithEmptyPasswords(IEnumerable<UserDto> userDtos)
        {
            return userDtos
                .Where(userDto => string.IsNullOrEmpty(userDto.UserPassword))
                .Select(userDto => new UserResultWithPasswordFormat(userDto));
        }

        private IEnumerable<UserResult> GetUsersWithPlaintextPasswords(IEnumerable<UserDto> userDtos)
        {
            return userDtos
                .Where(userDto => string.IsNullOrEmpty(userDto.UserPasswordFormat))
                .Select(userDto => new UserResult(userDto));
        }

        private ReportResults CompileResults(IEnumerable<UserResult> usersWithEmptyPasswords, IEnumerable<UserResult> usersWithPlaintextPasswords)
        {
            if (!usersWithEmptyPasswords.Any() && !usersWithPlaintextPasswords.Any())
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
                Data = new List<TableResult<UserResult>>()
            };

            var (emptyCount, usersWithEmptyPasswordsResult) = GetTableResult(usersWithEmptyPasswords, Metadata.Terms.TableTitles.EmptyPasswords);

            if (emptyCount > 0) errorReportResults.Data.Add(usersWithEmptyPasswordsResult);

            var (plaintextCount, usersWithPlaintextPasswordsResult) = GetTableResult(usersWithPlaintextPasswords, Metadata.Terms.TableTitles.PlaintextPasswords);

            if (plaintextCount > 0) errorReportResults.Data.Add(usersWithPlaintextPasswordsResult);

            errorReportResults.Summary = Metadata.Terms.ErrorSummary.With(new { emptyCount, plaintextCount });

            return errorReportResults;
        }

        private (int, TableResult<UserResult>) GetTableResult(IEnumerable<UserResult> userDtos, Term term)
        {
            var count = userDtos.Count();

            var tableResult = new TableResult<UserResult>
            {
                Name = term,
                Rows = userDtos
            };

            return (count, tableResult);
        }
    }
}