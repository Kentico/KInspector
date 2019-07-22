using System;
using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.ClassTableValidation.Models;

namespace KenticoInspector.Reports.ClassTableValidation
{
    public class Report : IReport, IWithMetadata<Labels>
    {
        private readonly IDatabaseService databaseService;
        private readonly IInstanceService instanceService;
        private readonly ILabelService labelService;

        public Report(IDatabaseService databaseService, IInstanceService instanceService, ILabelService labelService)
        {
            this.databaseService = databaseService;
            this.instanceService = instanceService;
            this.labelService = labelService;
        }

        public string Codename => nameof(ClassTableValidation);

        public IList<Version> CompatibleVersions => new List<Version>
        {
            new Version(10, 0),
            new Version(11, 0)
        };

        public IList<Version> IncompatibleVersions => new List<Version>();

        public IList<string> Tags => new List<string> {
            ReportTags.Health,
        };

        public Metadata<Labels> Metadata => labelService.GetMetadata<Labels>(Codename);

        public ReportResults GetResults()
        {
            var instance = instanceService.CurrentInstance;

            var instanceDetails = instanceService.GetInstanceDetails(instance);

            var tablesWithMissingClass = GetResultsForTables(instanceDetails);
            var classesWithMissingTable = GetResultsForClasses();

            return CompileResults(tablesWithMissingClass, classesWithMissingTable);
        }

        private ReportResults CompileResults(IEnumerable<TableWithNoClass> tablesWithMissingClass, IEnumerable<ClassWithNoTable> classesWithMissingTable)
        {
            var tableErrors = tablesWithMissingClass.Count();
            var tableResults = new TableResult<dynamic>()
            {
                Name = Metadata.Labels.DatabaseTablesWithMissingKenticoClasses,
                Rows = tablesWithMissingClass
            };

            var classErrors = classesWithMissingTable.Count();
            var classResults = new TableResult<dynamic>()
            {
                Name = Metadata.Labels.KenticoClassesWithMissingDatabaseTables,
                Rows = classesWithMissingTable
            };

            var totalErrors = tableErrors + classErrors;

            var results = new ReportResults
            {
                Type = ReportResultsType.TableList
            };

            results.Data.TableResults = tableResults;
            results.Data.ClassResults = classResults;

            switch (totalErrors)
            {
                case 0:
                    results.Status = ReportResultsStatus.Good;
                    results.Summary = Metadata.Labels.NoIssuesFound;
                    break;

                default:
                    results.Status = ReportResultsStatus.Error;
                    results.Summary = Metadata.Labels.CountIssueFound.With(new { count = totalErrors });
                    break;
            }

            return results;
        }

        private IEnumerable<ClassWithNoTable> GetResultsForClasses()
        {
            var classesWithMissingTable = databaseService.ExecuteSqlFromFile<ClassWithNoTable>(Scripts.ClassesWithNoTable);
            return classesWithMissingTable;
        }

        private IEnumerable<TableWithNoClass> GetResultsForTables(InstanceDetails instanceDetails)
        {
            var tablesWithMissingClass = databaseService.ExecuteSqlFromFile<TableWithNoClass>(Scripts.TablesWithNoClass);

            var tableWhitelist = GetTableWhitelist(instanceDetails.DatabaseVersion);
            if (tableWhitelist.Count > 0)
            {
                tablesWithMissingClass = tablesWithMissingClass.Where(t => !tableWhitelist.Contains(t.TableName)).ToList();
            }

            return tablesWithMissingClass;
        }

        private List<string> GetTableWhitelist(Version version)
        {
            var whitelist = new List<string>();

            if (version.Major >= 10)
            {
                whitelist.Add("CI_Migration");
            }

            return whitelist;
        }
    }
}