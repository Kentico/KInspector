using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.OnlineMarketingMacroAnalysis.Models;

using System;
using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Reports.OnlineMarketingMacroAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;

        public Report(IDatabaseService databaseService, IReportMetadataService reportMetadataService) : base(reportMetadataService)
        {
            this.databaseService = databaseService;
        }
        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12", "13");

        public override IList<string> Tags => new List<string> {
            ReportTags.Performance,
            ReportTags.OnlineMarketing
        };

        public override ReportResults GetResults()
        {
            var contactGroups = databaseService.ExecuteSqlFromFile<ContactGroupResult>(Scripts.GetManualContactGroupMacroConditions);
            var automationTriggers = databaseService.ExecuteSqlFromFile<AutomationTriggerResult>(Scripts.GetManualTimeBasedTriggerMacroConditions);
            var scoreRules = databaseService.ExecuteSqlFromFile<ScoreRuleResult>(Scripts.GetManualScoreRuleMacroConditions);
            if (!contactGroups.Any() && !automationTriggers.Any() && !scoreRules.Any())
            {
                return new ReportResults
                {
                    Status = ReportResultsStatus.Good,
                    Summary = Metadata.Terms.Good
                };
            }

            var totalIssues = contactGroups.Count() + automationTriggers.Count() + scoreRules.Count();
            var results = new ReportResults
            {
                Type = ReportResultsType.TableList,
                Status = ReportResultsStatus.Warning,
                Summary = Metadata.Terms.IssuesFound.With(new
                {
                    totalIssues
                })
            };
            var contactGroupResults = new TableResult<dynamic>()
            {
                Name = Metadata.Terms.ContactGroupTable,
                Rows = contactGroups
            };
            
            var automationTriggerResults = new TableResult<dynamic>()
            {
                Name = Metadata.Terms.AutomationTriggerTable,
                Rows = automationTriggers
            };

            var scoreRuleResults = new TableResult<dynamic>()
            {
                Name = Metadata.Terms.ScoreRuleTable,
                Rows = scoreRules
            };

            results.Data.AutomationTriggerTable = automationTriggerResults;
            results.Data.ContactGroupTable = contactGroupResults;
            results.Data.ScoreRuleTable = scoreRuleResults;

            return results;
        }
    }
}