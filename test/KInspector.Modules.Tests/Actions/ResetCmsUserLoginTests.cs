using KInspector.Actions.ResetCmsUserLogin;
using KInspector.Actions.ResetCmsUserLogin.Models;
using KInspector.Core.Constants;

using Moq;

using Newtonsoft.Json;

using NUnit.Framework;

using Action = KInspector.Actions.ResetCmsUserLogin.Action;

namespace KInspector.Tests.Common.Actions
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    [TestFixture(13)]
    public class ResetCmsUserLoginTests : AbstractActionTest<Action, Terms, Options>
    {
        private readonly Action _mockAction;

        public ResetCmsUserLoginTests(int majorVersion) : base(majorVersion)
        {
            _mockAction = new Action(_mockDatabaseService.Object, _mockModuleMetadataService.Object);
        }

        [Test]
        public async Task Should_NotModifyData_When_OptionsNull()
        {
            // Arrange
            var options = new Options { UserId = null };
            var tableResults = GetCleanTableResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsUser>(Scripts.GetAdministrators))
                .Returns(Task.FromResult(tableResults));

            // Act
            var optionJson = JsonConvert.SerializeObject(options);
            var results = await _mockAction.Execute(optionJson);

            // Assert
            Assert.That(results.TableResults.Any());
            Assert.That(results.TableResults.FirstOrDefault()?.Rows.Count() == 4);
            Assert.That(results.Status == ResultsStatus.Information);
            _mockDatabaseService.Verify(m => m.ExecuteNonQuery(Scripts.ResetAndEnableUser, It.IsAny<object>()), Times.Never());
        }

        [TestCase(0)]
        [TestCase(5)]
        public async Task Should_NotModifyData_When_InvalidOptions(int userId)
        {
            // Arrange
            var options = new Options { UserId = userId };
            var tableResults = GetCleanTableResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsUser>(Scripts.GetAdministrators))
                .Returns(Task.FromResult(tableResults));

            // Act
            var optionJson = JsonConvert.SerializeObject(options);
            var results = await _mockAction.Execute(optionJson);

            // Assert
            Assert.That(results.TableResults.Any());
            Assert.That(results.TableResults.FirstOrDefault()?.Rows.Count() == 4);
            Assert.That(results.Status == ResultsStatus.Error);
            _mockDatabaseService.Verify(m => m.ExecuteNonQuery(Scripts.ResetAndEnableUser, It.IsAny<object>()), Times.Never());
        }

        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public async Task Should_ModifyData_When_ValidOptions(int userId)
        {
            // Arrange
            var options = new Options { UserId = userId };
            var tableResults = GetCleanTableResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsUser>(Scripts.GetAdministrators))
                .Returns(Task.FromResult(tableResults));

            _mockDatabaseService
                .Setup(p => p.ExecuteNonQuery(Scripts.ResetAndEnableUser, It.IsAny<object>()))
                .Returns(Task.CompletedTask);

            // Act
            var optionJson = JsonConvert.SerializeObject(options);
            var results = await _mockAction.Execute(optionJson);

            // Assert
            Assert.That(results.TableResults.Any());
            Assert.That(results.TableResults.FirstOrDefault()?.Rows.Count() == 4);
            Assert.That(results.Status == ResultsStatus.Good);
            _mockDatabaseService.Verify(m => m.ExecuteNonQuery(Scripts.ResetAndEnableUser, It.IsAny<object>()), Times.Once());
        }

        private IEnumerable<CmsUser> GetCleanTableResults()
        {
            return new List<CmsUser>
            {
                new() {
                    UserID = 1,
                    Password = "",
                    Enabled = true
                },
                new() {
                    UserID = 2,
                    Password = "",
                    Enabled = false
                },
                new() {
                    UserID = 3,
                    Password = "testpassword2",
                    Enabled = true
                },
                new() {
                    UserID = 4,
                    Password = "testpassword2",
                    Enabled = false
                }
            };
        }
    }
}
