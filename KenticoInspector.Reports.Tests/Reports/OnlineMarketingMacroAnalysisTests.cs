using KenticoInspector.Core.Constants;
using KenticoInspector.Reports.OnlineMarketingMacroAnalysis;
using KenticoInspector.Reports.OnlineMarketingMacroAnalysis.Models;

using NUnit.Framework;

using System.Collections.Generic;

namespace KenticoInspector.Modules.Tests.Reports
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    [TestFixture(13)]
    public class OnlineMarketingMacroAnalysisTests : AbstractModuleTest<Report, Terms>
    {
        private readonly Report _mockReport;

        public OnlineMarketingMacroAnalysisTests(int majorVersion) : base(majorVersion)
        {
            _mockReport = new Report(_mockDatabaseService.Object, _mockModuleMetadataService.Object);
        }

        [Test]
        public void Should_ReturnGoodResult_WhenNoIssuesFound()
        {
            // Arrange
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<ContactGroupResult>(Scripts.GetManualContactGroupMacroConditions))
                .Returns(new ContactGroupResult[0]);
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<AutomationTriggerResult>(Scripts.GetManualTimeBasedTriggerMacroConditions))
                .Returns(new AutomationTriggerResult[0]);
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<ScoreRuleResult>(Scripts.GetManualScoreRuleMacroConditions))
                .Returns(new ScoreRuleResult[0]);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status == ResultsStatus.Good);
            Assert.That(results.Summary == _mockReport.Metadata.Terms.Good);
        }

        [Test]
        public void Should_ReturnUnoptimizedContactGroups_WhenSomeAreFound()
        {
            // Arrange
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<AutomationTriggerResult>(Scripts.GetManualTimeBasedTriggerMacroConditions))
                .Returns(new AutomationTriggerResult[0]);
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<ScoreRuleResult>(Scripts.GetManualScoreRuleMacroConditions))
                .Returns(new ScoreRuleResult[0]);

            var unoptimizedContactGroups = GetListOfContactGroups();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<ContactGroupResult>(Scripts.GetManualContactGroupMacroConditions))
                .Returns(unoptimizedContactGroups);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Data.ContactGroupTable.Rows.Count == 2);
            Assert.That(results.Status == ResultsStatus.Warning);
        }

        [Test]
        public void Should_ReturnUnoptimizedAutomationTriggers_WhenSomeAreFound()
        {
            // Arrange
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<ContactGroupResult>(Scripts.GetManualContactGroupMacroConditions))
                .Returns(new ContactGroupResult[0]);
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<ScoreRuleResult>(Scripts.GetManualScoreRuleMacroConditions))
                .Returns(new ScoreRuleResult[0]);

            var unoptimizedAutomationTriggers = GetListOfAutomationTriggers();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<AutomationTriggerResult>(Scripts.GetManualTimeBasedTriggerMacroConditions))
                .Returns(unoptimizedAutomationTriggers);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Data.AutomationTriggerTable.Rows.Count == 3);
            Assert.That(results.Status == ResultsStatus.Warning);
        }

        [Test]
        public void Should_ReturnUnoptimizedScoreRules_WhenSomeAreFound()
        {
            // Arrange
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<ContactGroupResult>(Scripts.GetManualContactGroupMacroConditions))
                .Returns(new ContactGroupResult[0]);
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<AutomationTriggerResult>(Scripts.GetManualTimeBasedTriggerMacroConditions))
                .Returns(new AutomationTriggerResult[0]);

            var unoptimizedScoreRules = GetListOfScoreRules();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<ScoreRuleResult>(Scripts.GetManualScoreRuleMacroConditions))
                .Returns(unoptimizedScoreRules);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Data.ScoreRuleTable.Rows.Count == 1);
            Assert.That(results.Status == ResultsStatus.Warning);
        }

        private IEnumerable<AutomationTriggerResult> GetListOfAutomationTriggers()
        {
            return new List<AutomationTriggerResult>
            {
                new AutomationTriggerResult
                {
                    ProcessName = "Process 1",
                    TriggerName = "Trigger 1",
                    Macro = "{%Contact.ContactCompanyName == \"a\"|(identity)GlobalAdministrator|(hash)54f49c07a82a0c646085ea04ab121406a20a83aa6c46670a3139e2afab8426d5%}"
                },
                new AutomationTriggerResult
                {
                    ProcessName = "Process 1",
                    TriggerName = "Trigger 2",
                    Macro = "{%Contact.ContactCompanyName == \"b\"|(identity)GlobalAdministrator|(hash)54f49c07a82a0c646085ea04ab121406a20a83aa6c46670a3139e2afab8426d5%}"
                },
                new AutomationTriggerResult
                {
                    ProcessName = "Process 2",
                    TriggerName = "Trigger 1",
                    Macro = "{%Contact.ContactAge == 42|(identity)GlobalAdministrator|(hash)54f49c07a82a0c646085ea04ab121406a20a83aa6c46670a3139e2afab8426d5%}"
                }
            };
        }

        private IEnumerable<ScoreRuleResult> GetListOfScoreRules()
        {
            return new List<ScoreRuleResult>
            {
                new ScoreRuleResult
                {
                    ScoreName = "Score 1",
                    RuleName = "Rule 1",
                    Macro = "<condition><macro><value>{%Contact.ContactAge == 42|(identity)GlobalAdministrator|(hash)8078b55cd2036a6590a123c9b0f0224a7d4efa0ee6d296c3a5444d2a982e1b2f%}</value></macro></condition>"
                }
            };
        }

        private IEnumerable<ContactGroupResult> GetListOfContactGroups()
        {
            return new List<ContactGroupResult>
            {
                new ContactGroupResult
                {
                    ContactGroup = "Group 1",
                    Macro = "{%Contact.ContactEmail.StartsWith(\"a\")|(identity)GlobalAdministrator|(hash)4dba4d29841dd536c94dadf53683801b89892c1dbee720f3627c6cc5bc77615b%}"
                },
                new ContactGroupResult
                {
                    ContactGroup = "Group 2",
                    Macro = "{%Contact.ContactAge == 42|(identity)GlobalAdministrator|(hash)4dba4d29841dd536c94dadf53683801b89892c1dbee720f3627c6cc5bc77615b%}"
                },
            };
        }
    }
}
