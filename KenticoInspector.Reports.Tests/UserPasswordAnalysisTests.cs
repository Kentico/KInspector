using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Reports.Tests.Helpers;
using KenticoInspector.Reports.UserPasswordAnalysis;
using KenticoInspector.Reports.UserPasswordAnalysis.Models;
using KenticoInspector.Reports.UserPasswordAnalysis.Models.Data;
using KenticoInspector.Reports.UserPasswordAnalysis.Models.Data.Results;

using NUnit.Framework;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    [TestFixture(13)]
    public class UserPasswordAnalysisTests : AbstractReportTest<Report, Terms>
    {
        private readonly Report mockReport;

        private IEnumerable<CmsUser> CmsUserWithoutIssues => new List<CmsUser>
        {
            new CmsUser
            {
                UserPassword = "ABAnAAA/1oSX1",
                UserPasswordFormat = "SHA1"
            }
        };

        private IEnumerable<CmsUser> CmsUserWithOneIssue => new List<CmsUser>
        {
            new CmsUser
            {
                UserPassword = "",
                UserPasswordFormat = "SHA1"
            }
        };

        private IEnumerable<CmsUser> CmsUserWithTwoIssues => new List<CmsUser>
        {
            new CmsUser
            {
                UserPassword = "password1",
                UserPasswordFormat = ""
            },
            new CmsUser
            {
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
        public void Should_ReturnGoodStatusAndGoodSummary_When_UserPasswordsHaveNoIssues()
        {
            // Arrange
            ArrangeDatabaseService(CmsUserWithoutIssues);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Good));
            Assert.That(results.Summary, Is.EqualTo(mockReport.Metadata.Terms.GoodSummary.ToString()));
        }

        [Test]
        public void Should_ReturnErrorStatusAndErrorSummary_When_UserPasswordsHaveTwoIssues()
        {
            // Arrange
            ArrangeDatabaseService(CmsUserWithTwoIssues);

            // Act
            var results = mockReport.GetResults();
            var resultsData = results.Data as List<TableResult<CmsUserResult>>;
            var firstResultRowCount = resultsData[0].Rows.Count();
            var secondResultRowCount = resultsData[1].Rows.Count();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Error));
            Assert.That(results.Summary, Is.EqualTo(mockReport.Metadata.Terms.ErrorSummary.ToString()));
            Assert.That(resultsData.Count, Is.EqualTo(2));
            Assert.That(firstResultRowCount, Is.EqualTo(1));
            Assert.That(secondResultRowCount, Is.EqualTo(1));
        }

        [Test]
        public void Should_ReturnErrorStatusAndErrorSummary_When_UserPasswordsHaveOneIssue()
        {
            // Arrange
            ArrangeDatabaseService(CmsUserWithOneIssue);

            // Act
            var results = mockReport.GetResults();
            var resultsData = results.Data as List<TableResult<CmsUserResult>>;
            var firstResultRowCount = resultsData[0].Rows.Count();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Error));
            Assert.That(results.Summary, Is.EqualTo(mockReport.Metadata.Terms.ErrorSummary.ToString()));
            Assert.That(resultsData.Count, Is.EqualTo(1));
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