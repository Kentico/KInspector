using KInspector.Core;
using KInspector.Core.Constants;
using KInspector.Core.Helpers;
using KInspector.Core.Models;
using KInspector.Core.Services.Interfaces;
using KInspector.Reports.DatabaseConsistencyCheck.Models;

using System.Data;

namespace KInspector.Reports.DatabaseConsistencyCheck
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;

        public Report(IDatabaseService databaseService, IModuleMetadataService moduleMetadataService) : base(moduleMetadataService)
        {
            this.databaseService = databaseService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12", "13");

        public override IList<string> Tags => new List<string> {
            ModuleTags.Health
        };

        public async override Task<ModuleResults> GetResults()
        {
#pragma warning disable 0618 // This is a special exemption as the results of CheckDB are unknown
            var checkDbResults = await databaseService.ExecuteSqlFromFileAsDataTable(Scripts.GetCheckDbResults);
#pragma warning restore 0618

            return CompileResults(checkDbResults);
        }

        private ModuleResults CompileResults(DataTable checkDbResults)
        {
            var hasIssues = checkDbResults.Rows.Count > 0;

            if (hasIssues)
            {
                var result = new ModuleResults
                {
                    Type = ResultsType.TableList,
                    Status = ResultsStatus.Error,
                    Summary = Metadata.Terms.CheckResultsTableForAnyIssues
                };
                result.TableResults.Add(new TableResult
                {
                    Name = "CHECKDB results",
                    Rows = checkDbResults.Rows.OfType<DataRow>()
                });

                return result;
            }
            else
            {
                return new ModuleResults
                {
                    Type = ResultsType.NoResults,
                    Status = ResultsStatus.Good,
                    Summary = Metadata.Terms.NoIssuesFound
                };
            }
        }
    }
}