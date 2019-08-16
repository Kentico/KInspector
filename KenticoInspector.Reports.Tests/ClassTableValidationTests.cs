using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Reports.ClassTableValidation;
using KenticoInspector.Reports.ClassTableValidation.Models;
using KenticoInspector.Reports.ClassTableValidation.Models.Data;
using KenticoInspector.Reports.Tests.Helpers;

using NUnit.Framework;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class ClassTableValidationTests : AbstractReportTest<Report, Terms>
    {
        private Report _mockReport;

        private List<CmsClass> CmsClassesWithTables => new List<CmsClass>();

        public ClassTableValidationTests(int majorVersion) : base(majorVersion)
        {
            _mockReport = new Report(_mockDatabaseService.Object, _mockInstanceService.Object, _mockReportMetadataService.Object);
        }

        [TestCase(Category = "No invalid classes or tables", TestName = "Database without invalid classes or tables produces a good result")]
        public void Should_ReturnGoodResult_When_DatabaseWithoutIssues()
        {
            // Arrange
            var tableResults = GetTableResultsWithoutIssues();

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<DatabaseTable>(Scripts.GetTablesWithMissingClass))
                .Returns(tableResults);

            var classResults = CmsClassesWithTables;

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsClass>(Scripts.GetCmsClassesWithMissingTable))
                .Returns(classResults);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Good));
        }

        [TestCase(Category = "Class with no table", TestName = "Database with a class with no table produces an error result and lists one class")]
        public void Should_ReturnErrorResult_When_DatabaseWithClassWithNoTable()
        {
            // Arrange
            var tableResults = GetTableResultsWithoutIssues();

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<DatabaseTable>(Scripts.GetTablesWithMissingClass))
                .Returns(tableResults);

            var classResults = CmsClassesWithTables;

            classResults.Add(new CmsClass
            {
                ClassDisplayName = "Has no table",
                ClassName = "HasNoTable",
                ClassTableName = "Custom_HasNoTable"
            });

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsClass>(Scripts.GetCmsClassesWithMissingTable))
                .Returns(classResults);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.GetAnonymousTableResult<TableResult<DatabaseTable>>("tableResults").Rows.Count(), Is.EqualTo(0));
            Assert.That(results.GetAnonymousTableResult<TableResult<CmsClass>>("classResults").Rows.Count(), Is.EqualTo(1));
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Error));
        }

        [TestCase(Category = "Table with no class", TestName = "Database with a table with no class produces an error result and lists one table")]
        public void Should_ReturnErrorResult_When_DatabaseWithTableWithNoClass()
        {
            // Arrange
            var tableResults = GetTableResultsWithoutIssues(false);
            tableResults.Add(new DatabaseTable
            {
                TableName = "HasNoClass"
            });

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<DatabaseTable>(Scripts.GetTablesWithMissingClass))
                .Returns(tableResults);

            var classResults = CmsClassesWithTables;

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsClass>(Scripts.GetCmsClassesWithMissingTable))
                .Returns(classResults);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.GetAnonymousTableResult<TableResult<DatabaseTable>>("tableResults").Rows.Count(), Is.EqualTo(1));
            Assert.That(results.GetAnonymousTableResult<TableResult<CmsClass>>("classResults").Rows.Count(), Is.EqualTo(0));
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Error));
        }

        private List<DatabaseTable> GetTableResultsWithoutIssues(bool includeWhitelistedTables = true)
        {
            var tableResults = new List<DatabaseTable>();

            if (includeWhitelistedTables && _mockInstanceDetails.DatabaseVersion.Major >= 10)
            {
                tableResults.Add(new DatabaseTable()
                {
                    TableName = "CI_Migration"
                });
            }

            return tableResults;
        }
    }
}