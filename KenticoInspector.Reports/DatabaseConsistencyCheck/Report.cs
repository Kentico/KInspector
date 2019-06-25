using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace KenticoInspector.Reports.DatabaseConsistencyCheck
{
    public class Report : IReport
    {
        readonly IDatabaseService _databaseService;
        readonly IInstanceService _instanceService;

        public Report(IDatabaseService databaseService, IInstanceService instanceService)
        {
            _databaseService = databaseService;
            _instanceService = instanceService;
        }

        public string Codename => "database-consistency-check";

        public IList<Version> CompatibleVersions => new List<Version> {
            new Version("10.0"),
            new Version("11.0"),
            new Version("12.0")
        };

        public IList<Version> IncompatibleVersions => new List<Version>();

        public string LongDescription => @"
            <p>As noted on <a href=""https://msdn.microsoft.com/en-us/library/ms176064.aspx"" target=""_blank"">MSDN</a>:</p>
            <p>Checks the logical and physical integrity of all the objects in the specified database by performing the following operations:</p>
            <ul>
                <li>Runs DBCC CHECKALLOC on the database.</li>
                <li>Runs DBCC CHECKTABLE on every table and view in the database.</li>
                <li>Runs DBCC CHECKCATALOG</a> on the database.</li>
                <li>Validates the contents of every indexed view in the database.</li>
                <li>Validates link-level consistency between table metadata and file system directories and files when storing<strong> varbinary(max)</strong> data in the file system using FILESTREAM.</li>
                <li class="">Validates the Service Broker data in the database.</li>
            </ul>";

        public string Name => "Database consistency check";

        public string ShortDescription => "Runs `DBCC CHECKDB` against the database to identify consistency issues.";

        public IList<string> Tags => new List<string> {
            ReportTags.Database,
            ReportTags.Health
        };

        public ReportResults GetResults(Guid InstanceGuid)
        {
            var instance = _instanceService.GetInstance(InstanceGuid);
            var instanceDetails = _instanceService.GetInstanceDetails(instance);
            _databaseService.ConfigureForInstance(instance);

#pragma warning disable 0618 // This is a special exemption as the results of CheckDB are unknown
            var checkDbResults = _databaseService.ExecuteSqlFromFileAsDataTable(Scripts.GetCheckDbResults);
#pragma warning restore 0618

            return CompileResults(checkDbResults);
        }

        private static ReportResults CompileResults(DataTable checkDbResults)
        {
            var hasIssues = checkDbResults.Rows.Count > 0;

            if (hasIssues)
            {
                return new ReportResults
                {
                    Type = ReportResultsType.Table,
                    Status = ReportResultsStatus.Error,
                    Summary = "Check results table for any issues",
                    Data = checkDbResults
                };
            }
            else
            {
                return new ReportResults
                {
                    Type = ReportResultsType.String,
                    Status = ReportResultsStatus.Good,
                    Summary = "No issues found",
                    Data = string.Empty
                };
            }
        }
    }
}
