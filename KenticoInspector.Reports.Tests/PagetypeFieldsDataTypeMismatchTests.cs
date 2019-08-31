using System.Collections.Generic;

using KenticoInspector.Core.Constants;
using KenticoInspector.Reports.PagetypeFieldsDataTypeMisMatch;
using KenticoInspector.Reports.PagetypeFieldsDataTypeMisMatch.Models;

using NUnit.Framework;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class PagetypeFieldsDataTypeMismatchTests : AbstractReportTest<Report, Terms>
    {
        private Report _mockReport;

        public PagetypeFieldsDataTypeMismatchTests(int majorVersion) : base(majorVersion)
        {
            _mockReport = new Report(_mockDatabaseService.Object, _mockInstanceService.Object, _mockReportMetadataService.Object);
        }

        [Test]
        public void Should_ReturnGoodResult_When_NoMismatchedFieldsExist()
        {
            // Arrange
            var fieldResults = GetCleanFieldResults();

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<ClassField>(Scripts.GetFieldsWithMismatchedTypes))
                .Returns(fieldResults);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Data.FieldResults.Rows.Count == 0);
            Assert.That(results.Status == ResultsStatus.Good);
        }

        [Test]
        public void Should_ReturnInfoResult_When_MismatchedFieldsExist()
        {
            // Arrange
            var fieldResults = GetMismatchedFieldsResults();

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<ClassField>(Scripts.GetFieldsWithMismatchedTypes))
                .Returns(fieldResults);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Data.FieldResults.Rows.Count == 2);
            Assert.That(results.Status == ResultsStatus.Information);
        }

        private List<ClassField> GetMismatchedFieldsResults()
        {
            var mismatchedFields = new List<ClassField>();

            mismatchedFields.Add(new ClassField()
            {
                PageType = "DancingGoatMvc.Article",
                FieldName = "ArticleText",
                DataType = "varchar"
            });

            mismatchedFields.Add(new ClassField()
            {
                PageType = "DancingGoatMvc.AboutUs",
                FieldName = "ArticleText",
                DataType = "int"
            });

            return mismatchedFields;
        }

        private List<ClassField> GetCleanFieldResults()
        {
            return new List<ClassField>();
        }
    }
}