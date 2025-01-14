using KInspector.Core.Constants;
using KInspector.Tests.Common.Helpers;
using KInspector.Reports.UserPasswordAnalysis;
using KInspector.Reports.UserPasswordAnalysis.Models;
using KInspector.Reports.UserPasswordAnalysis.Models.Data;

using NUnit.Framework;

namespace KInspector.Tests.Common.Reports
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    [TestFixture(13)]
    public class UserPasswordAnalysisTests : AbstractModuleTest<Report, Terms>
    {
        private readonly Report mockReport;

        private IEnumerable<CmsUser> CmsUserWithoutIssues => new List<CmsUser>
        {
            new() {
                UserPassword = "ABAnAAA/1oSX1",
                UserPasswordFormat = "SHA1"
            }
        };

        private IEnumerable<CmsUser> CmsUserWithOneIssue => new List<CmsUser>
        {
            new() {
                UserPassword = "",
                UserPasswordFormat = "SHA1"
            }
        };

        private IEnumerable<CmsUser> CmsUserWithTwoIssues => new List<CmsUser>
        {
            new() {
                UserPassword = "password1",
                UserPasswordFormat = ""
            },
            new() {
                UserPassword = "",
                UserPasswordFormat = "SHA1"
            }
        };

        public UserPasswordAnalysisTests(
            int majorVersion)
            : base(majorVersion)
        {
            mockReport = new Report(_mockDatabaseService.Object, _mockModuleMetadataService.Object);
        }

        [Test]
        public async Task Should_ReturnGoodStatusAndGoodSummary_When_UserPasswordsHaveNoIssues()
        {
            // Arrange
            ArrangeDatabaseService(CmsUserWithoutIssues);

            // Act
            var results = await mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Good));
            Assert.That(results.Summary, Is.EqualTo(mockReport.Metadata.Terms.GoodSummary?.ToString()));
        }

        [Test]
        public async Task Should_ReturnErrorStatusAndErrorSummary_When_UserPasswordsHaveTwoIssues()
        {
            // Arrange
            ArrangeDatabaseService(CmsUserWithTwoIssues);

            // Act
            var results = await mockReport.GetResults();
            var firstResultRowCount = results.TableResults[0].Rows.Count();
            var secondResultRowCount = results.TableResults[1].Rows.Count();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Error));
            Assert.That(results.Summary, Is.EqualTo(mockReport.Metadata.Terms.ErrorSummary?.ToString()));
            Assert.That(results.TableResults.Count, Is.EqualTo(2));
            Assert.That(firstResultRowCount, Is.EqualTo(1));
            Assert.That(secondResultRowCount, Is.EqualTo(1));
        }

        [Test]
        public async Task Should_ReturnErrorStatusAndErrorSummary_When_UserPasswordsHaveOneIssue()
        {
            // Arrange
            ArrangeDatabaseService(CmsUserWithOneIssue);

            // Act
            var results = await mockReport.GetResults();
            var firstResultRowCount = results.TableResults.FirstOrDefault()?.Rows.Count();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Error));
            Assert.That(results.Summary, Is.EqualTo(mockReport.Metadata.Terms.ErrorSummary?.ToString()));
            Assert.That(results.TableResults.Count, Is.EqualTo(1));
            Assert.That(firstResultRowCount, Is.EqualTo(1));
        }

        private void ArrangeDatabaseService(
            IEnumerable<CmsUser> cmsUserTable)
        {
            _mockDatabaseService.SetupExecuteSqlFromFileWithListParameter(
                Scripts.GetEnabledAndNotExternalUsers,
                nameof(Report.ExcludedUserNames),
                Report.ExcludedUserNames,
                cmsUserTable
                );
        }
    }
}