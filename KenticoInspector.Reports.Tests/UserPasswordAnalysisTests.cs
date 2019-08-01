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
    public class UserPasswordAnalysisTests : AbstractReportTest<Report, Terms>
    {
        private readonly Report mockReport;

        private IEnumerable<UserDto> UserDtoTableWithoutIssues => new List<UserDto>
        {
            new UserDto
            {
                UserPassword = "ABAnAAA/1oSX1",
                UserPasswordFormat = "SHA1"
            }
        };

        private IEnumerable<UserDto> UserDtoTableWithOneIssue => new List<UserDto>
        {
            new UserDto
            {
                UserPassword = "",
                UserPasswordFormat = "SHA1"
            }
        };

        private IEnumerable<UserDto> UserDtoTableWithTwoIssues => new List<UserDto>
        {
            new UserDto
            {
                UserPassword = "password1",
                UserPasswordFormat = ""
            },
            new UserDto
            {
                UserPassword = "",
                UserPasswordFormat = "SHA1"
            }
        };

        public UserPasswordAnalysisTests(int majorVersion) : base(majorVersion)
        {
            mockReport = new Report(_mockDatabaseService.Object, _mockReportMetadataService.Object);
        }

        [Test]
        public void Should_ReturnGoodStatusAndGoodSummary_WhenUserPasswordsHaveNoIssues()
        {
            // Arrange
            ArrangeDatabaseService(UserDtoTableWithoutIssues);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Good));

            Assert.That(results.Summary, Is.EqualTo(mockReport.Metadata.Terms.GoodSummary.ToString()));
        }

        [Test]
        public void Should_ReturnErrorStatusAndErrorSummary_WhenUserPasswordsHaveTwoIssues()
        {
            // Arrange
            ArrangeDatabaseService(UserDtoTableWithTwoIssues);

            // Act
            var results = mockReport.GetResults();
            var resultsData = results.Data as List<TableResult<UserResult>>;

            // Assert
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Error));

            Assert.That(results.Summary, Is.EqualTo(mockReport.Metadata.Terms.ErrorSummary.ToString()));

            Assert.That(resultsData.Count, Is.EqualTo(2));
            Assert.That(resultsData[0].Rows.Count(), Is.EqualTo(1));
            Assert.That(resultsData[1].Rows.Count(), Is.EqualTo(1));
        }

        [Test]
        public void Should_ReturnErrorStatusAndErrorSummary_WhenUserPasswordsHaveOneIssue()
        {
            // Arrange
            ArrangeDatabaseService(UserDtoTableWithOneIssue);

            // Act
            var results = mockReport.GetResults();
            var resultsData = results.Data as List<TableResult<UserResult>>;

            // Assert
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Error));

            Assert.That(results.Summary, Is.EqualTo(mockReport.Metadata.Terms.ErrorSummary.ToString()));

            Assert.That(resultsData.Count, Is.EqualTo(1));
            Assert.That(resultsData[0].Rows.Count(), Is.EqualTo(1));
        }

        private void ArrangeDatabaseService(IEnumerable<UserDto> userDtoTable)
        {
            _mockDatabaseService.SetupExecuteSqlFromFile(Scripts.GetEnabledAndNotExternalUsers, userDtoTable);
        }
    }
}