using KInspector.Core;
using KInspector.Core.Constants;
using KInspector.Core.Helpers;
using KInspector.Core.Models;
using KInspector.Core.Services.Interfaces;
using KInspector.Reports.OnlineMarketingMacroAnalysis.Models;

namespace KInspector.Reports.OnlineMarketingMacroAnalysis
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
            ModuleTags.Performance,
            ModuleTags.OnlineMarketing
        };

        public async override Task<ModuleResults> GetResults()
        {
            var contactGroups = await databaseService.ExecuteSqlFromFile<ContactGroupResult>(Scripts.GetManualContactGroupMacroConditions);
            var automationTriggers = await databaseService.ExecuteSqlFromFile<AutomationTriggerResult>(Scripts.GetManualTimeBasedTriggerMacroConditions);
            var scoreRules = await databaseService.ExecuteSqlFromFile<ScoreRuleResult>(Scripts.GetManualScoreRuleMacroConditions);
            if (!contactGroups.Any() && !automationTriggers.Any() && !scoreRules.Any())
            {
                return new ModuleResults
                {
                    Status = ResultsStatus.Good,
                    Summary = Metadata.Terms.Good,
                    Type = ResultsType.NoResults
                };
            }

            var totalIssues = contactGroups.Count() + automationTriggers.Count() + scoreRules.Count();
            var results = new ModuleResults
            {
                Type = ResultsType.TableList,
                Status = ResultsStatus.Warning,
                Summary = Metadata.Terms.IssuesFound?.With(new
                {
                    totalIssues
                })
            };
            results.TableResults.Add(new TableResult
            {
                Name = Metadata.Terms.ContactGroupTable,
                Rows = contactGroups
            });

            results.TableResults.Add(new TableResult
            {
                Name = Metadata.Terms.AutomationTriggerTable,
                Rows = automationTriggers
            });

            results.TableResults.Add(new TableResult
            {
                Name = Metadata.Terms.ScoreRuleTable,
                Rows = scoreRules
            });

            return results;
        }
    }
}