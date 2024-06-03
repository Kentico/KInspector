using KInspector.Actions.DisableWebFarmServers;
using KInspector.Actions.DisableWebFarmServers.Models;
using KInspector.Core.Constants;

using Moq;

using Newtonsoft.Json;

using NUnit.Framework;

using Action = KInspector.Actions.DisableWebFarmServers.Action;

namespace KInspector.Tests.Common.Actions
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    [TestFixture(13)]
    public class DisableWebFarmServersTests : AbstractActionTest<Action, Terms, Options>
    {
        private readonly Action _mockAction;

        public DisableWebFarmServersTests(int majorVersion) : base(majorVersion)
        {
            _mockAction = new Action(_mockDatabaseService.Object, _mockModuleMetadataService.Object);
        }

        [Test]
        public async Task Should_NotModifyData_When_OptionsNull()
        {
            // Arrange
            var options = new Options { ServerId = null };
            var tableResults = GetCleanTableResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<WebFarmServer>(Scripts.GetWebFarmServerSummary))
                .Returns(Task.FromResult(tableResults));

            // Act
            var optionJson = JsonConvert.SerializeObject(options);
            var results = await _mockAction.Execute(optionJson);

            // Assert
            Assert.That(results.TableResults.Any());
            Assert.That(results.TableResults.FirstOrDefault()?.Rows.Count() == 2);
            Assert.That(results.Status == ResultsStatus.Information);
            _mockDatabaseService.Verify(m => m.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<object>()), Times.Never());
        }

        [TestCase(0)]
        [TestCase(3)]
        public async Task Should_NotModifyData_When_InvalidOptions(int serverId)
        {
            // Arrange
            var options = new Options { ServerId = serverId };
            var tableResults = GetCleanTableResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<WebFarmServer>(Scripts.GetWebFarmServerSummary))
                .Returns(Task.FromResult(tableResults));

            // Act
            var optionJson = JsonConvert.SerializeObject(options);
            var results = await _mockAction.Execute(optionJson);

            // Assert
            Assert.That(results.TableResults.Any());
            Assert.That(results.TableResults.FirstOrDefault()?.Rows.Count() == 2);
            Assert.That(results.Status == ResultsStatus.Error);
            _mockDatabaseService.Verify(m => m.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<object>()), Times.Never());
        }

        [Test]
        public async Task Should_ModifyData_When_ValidOptions()
        {
            // Arrange
            var options = new Options { ServerId = 1 };
            var tableResults = GetCleanTableResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<WebFarmServer>(Scripts.GetWebFarmServerSummary))
                .Returns(Task.FromResult(tableResults));

            _mockDatabaseService
                .Setup(p => p.ExecuteNonQuery(Scripts.DisableServer, It.IsAny<object>()))
                .Returns(Task.CompletedTask);

            // Act
            var optionJson = JsonConvert.SerializeObject(options);
            var results = await _mockAction.Execute(optionJson);

            // Assert
            Assert.That(results.TableResults.Any());
            Assert.That(results.TableResults.FirstOrDefault()?.Rows.Count() == 2);
            Assert.That(results.Status == ResultsStatus.Good);
            _mockDatabaseService.Verify(m => m.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<object>()), Times.Once());
        }

        private IEnumerable<WebFarmServer> GetCleanTableResults()
        {
            return new List<WebFarmServer>
            {
                new() {
                    ID = 1,
                    Enabled = true
                },
                new() {
                    ID = 2,
                    Enabled = false
                }
            };
        }
    }
}
