using KInspector.Actions.DisableSmtpServers;
using KInspector.Actions.DisableSmtpServers.Models;
using KInspector.Core.Constants;

using Moq;

using Newtonsoft.Json;

using NUnit.Framework;

using Action = KInspector.Actions.DisableSmtpServers.Action;

namespace KInspector.Tests.Common.Actions
{
    [TestFixture(12)]
    [TestFixture(13)]
    public class DisableSmtpServersTests : AbstractActionTest<Action, Terms, Options>
    {
        private readonly Action _mockAction;

        public DisableSmtpServersTests(int majorVersion) : base(majorVersion)
        {
            _mockAction = new Action(_mockDatabaseService.Object, _mockModuleMetadataService.Object);
        }

        [Test]
        public async Task Should_NotModifyData_When_OptionsNull()
        {
            // Arrange
            var options = new Options { ServerId = null, SiteId = null };
            var settingsResults = GetCleanSettingsResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<SmtpFromSettings>(Scripts.GetSmtpFromSettingsKeys))
                .Returns(Task.FromResult(settingsResults));

            var smtpResults = GetCleanSmtpResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<SmtpFromSmtpServers>(Scripts.GetSmtpFromSmtpServers))
                .Returns(Task.FromResult(smtpResults));

            // Act
            var optionJson = JsonConvert.SerializeObject(options);
            var results = await _mockAction.Execute(optionJson);
            var smtpTable = results.TableResults.FirstOrDefault(t => t.Name?.Equals(_mockAction.Metadata.Terms.ServersFromSmtpTable) ?? false);
            var settingsTable = results.TableResults.FirstOrDefault(t => t.Name?.Equals(_mockAction.Metadata.Terms.ServersFromSettingsTable) ?? false);

            // Assert
            Assert.That(smtpTable, Is.Not.Null);
            Assert.That(settingsTable, Is.Not.Null);
            Assert.That(settingsTable?.Rows.Count() == 3);
            Assert.That(smtpTable?.Rows.Count() == 2);
            Assert.That(results.Status == ResultsStatus.Information);
            _mockDatabaseService.Verify(m => m.ExecuteSqlFromFileGeneric(It.IsAny<string>()), Times.Never());
        }

        [TestCase(1, 1)]
        [TestCase(2, null)]
        [TestCase(5, null)]
        [TestCase(null, 1)]
        [TestCase(null, 5)]
        public async Task Should_NotModifyData_When_InvalidOptions(int serverId, int siteId)
        {
            // Arrange
            var options = new Options { ServerId = serverId, SiteId = siteId };
            var settingsResults = GetCleanSettingsResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<SmtpFromSettings>(Scripts.GetSmtpFromSettingsKeys))
                .Returns(Task.FromResult(settingsResults));

            var smtpResults = GetCleanSmtpResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<SmtpFromSmtpServers>(Scripts.GetSmtpFromSmtpServers))
                .Returns(Task.FromResult(smtpResults));

            // Act
            var optionJson = JsonConvert.SerializeObject(options);
            var results = await _mockAction.Execute(optionJson);
            var smtpTable = results.TableResults.FirstOrDefault(t => t.Name?.Equals(_mockAction.Metadata.Terms.ServersFromSmtpTable) ?? false);
            var settingsTable = results.TableResults.FirstOrDefault(t => t.Name?.Equals(_mockAction.Metadata.Terms.ServersFromSettingsTable) ?? false);

            // Assert
            Assert.That(smtpTable, Is.Not.Null);
            Assert.That(settingsTable, Is.Not.Null);
            Assert.That(settingsTable?.Rows.Count() == 3);
            Assert.That(smtpTable?.Rows.Count() == 2);
            Assert.That(results.Status == ResultsStatus.Error);
            _mockDatabaseService.Verify(m => m.ExecuteSqlFromFileGeneric(It.IsAny<string>()), Times.Never());
        }

        [TestCase(0)]
        [TestCase(2)]
        public async Task Should_ModifySiteSettings_When_ValidOptions(int siteId)
        {
            // Arrange
            var options = new Options { SiteId = siteId };
            var settingsResults = GetCleanSettingsResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<SmtpFromSettings>(Scripts.GetSmtpFromSettingsKeys))
                .Returns(Task.FromResult(settingsResults));

            var smtpResults = GetCleanSmtpResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<SmtpFromSmtpServers>(Scripts.GetSmtpFromSmtpServers))
                .Returns(Task.FromResult(smtpResults));

            _mockDatabaseService
                .Setup(p => p.ExecuteNonQuery(Scripts.DisableSiteSmtpServer, It.IsAny<object>()))
                .Returns(Task.CompletedTask);

            // Act
            var optionJson = JsonConvert.SerializeObject(options);
            var results = await _mockAction.Execute(optionJson);
            var smtpTable = results.TableResults.FirstOrDefault(t => t.Name?.Equals(_mockAction.Metadata.Terms.ServersFromSmtpTable) ?? false);
            var settingsTable = results.TableResults.FirstOrDefault(t => t.Name?.Equals(_mockAction.Metadata.Terms.ServersFromSettingsTable) ?? false);

            // Assert
            Assert.That(smtpTable, Is.Not.Null);
            Assert.That(settingsTable, Is.Not.Null);
            Assert.That(settingsTable?.Rows.Count() == 3);
            Assert.That(smtpTable?.Rows.Count() == 2);
            Assert.That(results.Status == ResultsStatus.Good);
            _mockDatabaseService.Verify(m => m.ExecuteNonQuery(Scripts.DisableSiteSmtpServer, It.IsAny<object>()), Times.Once());
        }

        [Test]
        public async Task Should_ModifySmtpServer_When_ValidOptions()
        {
            // Arrange
            var options = new Options { ServerId = 1 };
            var settingsResults = GetCleanSettingsResults().AsEnumerable();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<SmtpFromSettings>(Scripts.GetSmtpFromSettingsKeys))
                .Returns(Task.FromResult(settingsResults));

            var smtpResults = GetCleanSmtpResults().AsEnumerable();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<SmtpFromSmtpServers>(Scripts.GetSmtpFromSmtpServers))
                .Returns(Task.FromResult(smtpResults));

            _mockDatabaseService
                .Setup(p => p.ExecuteNonQuery(Scripts.DisableSmtpServer, It.IsAny<object>()))
                .Returns(Task.CompletedTask);

            // Act
            var optionJson = JsonConvert.SerializeObject(options);
            var results = await _mockAction.Execute(optionJson);
            var smtpTable = results.TableResults.FirstOrDefault(t => t.Name?.Equals(_mockAction.Metadata.Terms.ServersFromSmtpTable) ?? false);
            var settingsTable = results.TableResults.FirstOrDefault(t => t.Name?.Equals(_mockAction.Metadata.Terms.ServersFromSettingsTable) ?? false);

            // Assert
            Assert.That(smtpTable, Is.Not.Null);
            Assert.That(settingsTable, Is.Not.Null);
            Assert.That(settingsTable?.Rows.Count() == 3);
            Assert.That(smtpTable?.Rows.Count() == 2);
            Assert.That(results.Status == ResultsStatus.Good);
            _mockDatabaseService.Verify(m => m.ExecuteNonQuery(Scripts.DisableSmtpServer, It.IsAny<object>()), Times.Once());
        }

        private IEnumerable<SmtpFromSmtpServers> GetCleanSmtpResults()
        {
            return new List<SmtpFromSmtpServers>
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

        private IEnumerable<SmtpFromSettings> GetCleanSettingsResults()
        {
            return new List<SmtpFromSettings>
            {
                new() {
                    SiteID = 0,
                    Server = "localhost"
                },
                new() {
                    SiteID = 1,
                    Server = "mysite.com.disabled"
                },
                new() {
                    SiteID = 2,
                    Server = "othersite.com"
                }
            };
        }
    }
}
