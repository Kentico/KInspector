using System;
using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.ClassTableValidation.Models;
using KenticoInspector.Reports.ClassTableValidation.Models.Data;

namespace KenticoInspector.Reports.ClassTableValidation
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;
        private readonly IInstanceService instanceService;

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11");

        public override IList<string> Tags => new List<string>
        {
            ReportTags.Health
        };

        public Report(
            IDatabaseService databaseService,
            IInstanceService instanceService,
            IReportMetadataService reportMetadataService
            ) : base(reportMetadataService)
        {
            this.databaseService = databaseService;
            this.instanceService = instanceService;
        }

        public override ReportResults GetResults()
        {
            var instance = instanceService.CurrentInstance;

            var instanceDetails = instanceService.GetInstanceDetails(instance);

            var tableWhitelist = GetTableWhitelist(instanceDetails.DatabaseVersion);

            var tablesWithMissingClass = databaseService.ExecuteSqlFromFile<DatabaseTable>(
                Scripts.GetTablesWithMissingClass
            );

            var tablesWithMissingClassNotInWhitelist = GetTablesNotInWhitelist(tablesWithMissingClass, tableWhitelist);

            var cmsClassesWithMissingTable = databaseService.ExecuteSqlFromFile<CmsClass>(
                Scripts.GetCmsClassesWithMissingTable
            );

            return CompileResults(tablesWithMissingClassNotInWhitelist, cmsClassesWithMissingTable);
        }

        private static IEnumerable<string> GetTableWhitelist(Version version)
        {
            var whitelist = new List<string>();

            if (version.Major >= 10)
            {
                whitelist.Add("CI_Migration");
            }

            return whitelist;
        }

        private static IEnumerable<DatabaseTable> GetTablesNotInWhitelist(
            IEnumerable<DatabaseTable> tablesWithMissingClass,
            IEnumerable<string> tableWhitelist
            )
        {
            if (tableWhitelist.Any())
            {
                return tablesWithMissingClass
                    .Where(t => !tableWhitelist.Contains(t.TableName))
                    .ToList();
            }

            return tablesWithMissingClass;
        }

        private ReportResults CompileResults(
            IEnumerable<DatabaseTable> tablesWithMissingClass,
            IEnumerable<CmsClass> cmsClassesWithMissingTable
            )
        {
            if (!tablesWithMissingClass.Any() && !cmsClassesWithMissingTable.Any())
            {
                return new ReportResults()
                {
                    Status = ReportResultsStatus.Good,
                    Summary = Metadata.Terms.Summaries.Good
                };
            }

            var tableErrors = tablesWithMissingClass.Count();

            var tableResults = new TableResult<DatabaseTable>
            {
                Name = Metadata.Terms.TableTitles.DatabaseTablesWithMissingKenticoClasses,
                Rows = tablesWithMissingClass
            };

            var classErrors = cmsClassesWithMissingTable.Count();

            var classResults = new TableResult<CmsClass>
            {
                Name = Metadata.Terms.TableTitles.KenticoClassesWithMissingDatabaseTables,
                Rows = cmsClassesWithMissingTable
            };

            var totalErrors = tableErrors + classErrors;

            return new ReportResults
            {
                Data = new
                {
                    tableResults,
                    classResults
                },
                Status = ReportResultsStatus.Error,
                Summary = Metadata.Terms.Summaries.Error.With(new { totalErrors }),
                Type = ReportResultsType.TableList
            };
        }
    }
}