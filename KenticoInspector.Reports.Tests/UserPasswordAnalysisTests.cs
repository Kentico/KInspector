using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Models.Results;
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
    public class UserPasswordAnalysisTests : AbstractReportTest<Report, Terms>
    {
        private readonly Report mockReport;

        private IEnumerable<CmsUser> CmsUsersWithoutIssues => new List<CmsUser>
        {
            new CmsUser
            {
                UserPassword = "ABAnAAA/1oSX1",
                UserPasswordFormat = "SHA1"
            }
        };

        private IEnumerable<CmsUser> CmsUsersWithOneIssue => new List<CmsUser>
        {
            new CmsUser
            {
                UserPassword = "",
                UserPasswordFormat = "SHA1"
            }
        };

        private IEnumerable<CmsUser> CmsUsersWithTwoIssues => new List<CmsUser>
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
            mockReport = new Report(_mockDatabaseService.Object, _mockReportMetadataService.Object);
        }

        [Test]
        public void Should_ReturnGoodResult_When_UserPasswordsHaveNoIssues()
        {
            // Arrange
            ArrangeDatabaseService(CmsUsersWithoutIssues);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Good));

            Assert.That(results.Summary, Is.EqualTo(mockReport.Metadata.Terms.GoodSummary.ToString()));
        }

        [Test]
        public void Should_ReturnErrorResult_When_UserPasswordsHaveTwoIssues()
        {
            // Arrange
            ArrangeDatabaseService(CmsUsersWithTwoIssues);

            // Act
            var results = mockReport.GetResults();
            var cmsUserResultWithPasswordFormat = results.Data.First<TableResult<CmsUserResultWithPasswordFormat>>().Rows.Count();
            var cmsUserResult = results.Data.First<TableResult<CmsUserResult>>().Rows.Count();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Error));

            Assert.That(results.Summary, Is.EqualTo(mockReport.Metadata.Terms.ErrorSummary.ToString()));

            Assert.That(cmsUserResultWithPasswordFormat, Is.EqualTo(1));

            Assert.That(cmsUserResult, Is.EqualTo(1));
        }

        [Test]
        public void Should_ReturnErrorResult_When_UserPasswordsHaveOneIssue()
        {
            // Arrange
            ArrangeDatabaseService(CmsUsersWithOneIssue);

            // Act
            var results = mockReport.GetResults();
            var cmsUserResultWithPasswordFormat = results.Data.First<TableResult<CmsUserResultWithPasswordFormat>>().Rows.Count();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Error));

            Assert.That(results.Summary, Is.EqualTo(mockReport.Metadata.Terms.ErrorSummary.ToString()));

            Assert.That(cmsUserResultWithPasswordFormat, Is.EqualTo(1));
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