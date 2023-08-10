using KenticoInspector.Actions.WebFarmServerSummary;
using KenticoInspector.Actions.WebFarmServerSummary.Models;
using KenticoInspector.Core.Constants;

using Moq;

using Newtonsoft.Json;

using NUnit.Framework;

using System.Collections.Generic;

namespace KenticoInspector.Modules.Tests.Actions
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    [TestFixture(13)]
    public class WebFarmServerSummaryStatus : AbstractActionTest<Action, Terms, Options>
    {
        private readonly Action _mockAction;

        public WebFarmServerSummaryStatus(int majorVersion) : base(majorVersion)
        {
            _mockAction = new Action(_mockDatabaseService.Object, _mockModuleMetadataService.Object);
        }

        [Test]
        public void Should_NotModifyData_When_OptionsNull()
        {
            // Arrange
            var options = new Options { ServerId = null };
            var tableResults = GetCleanTableResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<WebFarmServer>(Scripts.GetWebFarmServerSummary))
                .Returns(tableResults);

            // Act
            var optionJson = JsonConvert.SerializeObject(options);
            var results = _mockAction.Execute(optionJson);

            // Assert
            Assert.That(results.Data.Rows.Count == 2);
            Assert.That(results.Status == ResultsStatus.Information);
            _mockDatabaseService.Verify(m => m.ExecuteSqlFromFileGeneric(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void Should_NotModifyData_When_InvalidOptions()
        {
            // Arrange
            var options = new Options { ServerId = 5 };
            var tableResults = GetCleanTableResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<WebFarmServer>(Scripts.GetWebFarmServerSummary))
                .Returns(tableResults);

            // Act
            var optionJson = JsonConvert.SerializeObject(options);
            var results = _mockAction.Execute(optionJson);

            // Assert
            Assert.That(results.Data.Rows.Count == 2);
            Assert.That(results.Status == ResultsStatus.Error);
            _mockDatabaseService.Verify(m => m.ExecuteSqlFromFileGeneric(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void Should_ModifyData_When_ValidOptions()
        {
            // Arrange
            var options = new Options { ServerId = 1 };
            var tableResults = GetCleanTableResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<WebFarmServer>(Scripts.GetWebFarmServerSummary))
                .Returns(tableResults);

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFileGeneric(Scripts.DisableServer, It.IsAny<object>()))
                .Returns(It.IsAny<IEnumerable<Dictionary<string, object>>>());

            // Act
            var optionJson = JsonConvert.SerializeObject(options);
            var results = _mockAction.Execute(optionJson);

            // Assert
            Assert.That(results.Data.Rows.Count == 2);
            Assert.That(results.Status == ResultsStatus.Good);
            _mockDatabaseService.Verify(m => m.ExecuteSqlFromFileGeneric(It.IsAny<string>(), It.IsAny<object>()), Times.Once());
        }

        private List<WebFarmServer> GetCleanTableResults()
        {
            return new List<WebFarmServer>
            {
                new WebFarmServer
                {
                    ID = 1,
                    Enabled = true
                },
                new WebFarmServer
                {
                    ID = 2,
                    Enabled = false
                }
            };
        }
    }
}
