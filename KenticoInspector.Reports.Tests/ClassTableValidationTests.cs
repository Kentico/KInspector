using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models.Results;
using KenticoInspector.Reports.ClassTableValidation;
using KenticoInspector.Reports.ClassTableValidation.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class ClassTableValidationTests : AbstractReportTest<Report, Terms>
    {
        private Report _mockReport;

        private List<ClassWithNoTable> CleanClassResults => new List<ClassWithNoTable>();

        public ClassTableValidationTests(int majorVersion) : base(majorVersion)
        {
            _mockReport = new Report(_mockDatabaseService.Object, _mockInstanceService.Object, _mockReportMetadataService.Object);
        }

        [Test]
        public void Should_ReturnGoodResult_When_DatabaseWithoutIssues()
        {
            // Arrange
            var tableResults = GetTableResultsWithoutIssues();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<TableWithNoClass>(Scripts.TablesWithNoClass))
                .Returns(tableResults);

            var classResults = CleanClassResults;

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<ClassWithNoTable>(Scripts.ClassesWithNoTable))
                .Returns(classResults);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Good));
        }

        [Test]
        public void Should_ReturnErrorResult_When_DatabaseWithClassWithNoTable()
        {
            // Arrange
            var tableResults = GetTableResultsWithoutIssues();

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<TableWithNoClass>(Scripts.TablesWithNoClass))
                .Returns(tableResults);

            var classResults = CleanClassResults;

            classResults.Add(new ClassWithNoTable
            {
                ClassDisplayName = "Has no table",
                ClassName = "HasNoTable",
                ClassTableName = "Custom_HasNoTable"
            });

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<ClassWithNoTable>(Scripts.ClassesWithNoTable))
                .Returns(classResults);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Error));
            Assert.That(results.Data.First<TableResult<ClassWithNoTable>>().Rows.Count(), Is.EqualTo(1));
            Assert.That(results.Data.First<TableResult<TableWithNoClass>>().Rows.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Should_ReturnErrorResult_When_DatabaseWithTableWithNoClass()
        {
            // Arrange
            var tableResults = GetTableResultsWithoutIssues(false);
            tableResults.Add(new TableWithNoClass
            {
                TableName = "HasNoClass"
            });

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<TableWithNoClass>(Scripts.TablesWithNoClass))
                .Returns(tableResults);

            var classResults = CleanClassResults;

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<ClassWithNoTable>(Scripts.ClassesWithNoTable))
                .Returns(classResults);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Data.First<TableResult<ClassWithNoTable>>().Rows.Count(), Is.EqualTo(0));
            Assert.That(results.Data.First<TableResult<TableWithNoClass>>().Rows.Count(), Is.EqualTo(1));
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Error));
        }

        private List<TableWithNoClass> GetTableResultsWithoutIssues(bool includeWhitelistedTables = true)
        {
            var tableResults = new List<TableWithNoClass>();

            if (includeWhitelistedTables && _mockInstanceDetails.DatabaseVersion.Major >= 10)
            {
                tableResults.Add(new TableWithNoClass() { TableName = "CI_Migration" });
            }

            return tableResults;
        }
    }
}