using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.Tests.MockHelpers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture]
    public class ClassTableValidationTests
    {
        private Mock<IInstanceService> _mockInstanceService;
        private Mock<IDatabaseService> _mockDatabaseService;

        [SetUp]
        public void Setup()
        {
        }


        [TestCase(9)]
        [TestCase(10)]
        [TestCase(11)]
        [TestCase(12)]
        public void Should_ReturnCleanResult_When_DatabaseIsClean(int majorVersion)
        {
            // Arrange
            var instance = MockInstances.Get(majorVersion);
            var instanceDetails = MockInstanceDetails.Get(majorVersion, instance);
            var tableResults = new List<TablesResult>();
            var classResults = new List<ClassesResult>();

            if (majorVersion >= 10)
            {
                tableResults.Add(new TablesResult() { TableName = "CI_Migration" });
            }

            _mockInstanceService = MockInstanceServiceHelper.SetupInstanceService(instance, instanceDetails);
            _mockDatabaseService = MockDatabaseServiceHelper.SetupMockDatabaseService(instance);
            
            _mockDatabaseService.Setup(p => p.ExecuteSqlFromFile<TablesResult>(ClassTableValidationScripts.TablesWithMissingClasses))
                .Returns(tableResults);
            _mockDatabaseService.Setup(p => p.ExecuteSqlFromFile<ClassesResult>(ClassTableValidationScripts.ClassesWithMissingTables))
                .Returns(new List<ClassesResult>());

            // Act
            var report = new ClassTableValidation(_mockDatabaseService.Object, _mockInstanceService.Object);
            var results = report.GetResults(instance.Guid);

            // Assert
            Assert.That(results.Data.TableResults.Rows.Count == 0);
            Assert.That(results.Status == ReportResultsStatus.Good.ToString());
        }
    }
}