﻿using KenticoInspector.Actions.GlobalAdminSummary;
using KenticoInspector.Actions.GlobalAdminSummary.Models;
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
    public class GlobalAdminSummaryTests : AbstractActionTest<Action, Terms, Options>
    {
        private readonly Action _mockAction;

        public GlobalAdminSummaryTests(int majorVersion) : base(majorVersion)
        {
            _mockAction = new Action(_mockDatabaseService.Object, _mockModuleMetadataService.Object);
        }

        [Test]
        public void Should_NotModifyData_When_OptionsNull()
        {
            // Arrange
            var options = new Options { UserId = null };
            var tableResults = GetCleanTableResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsUser>(Scripts.GetAdministrators))
                .Returns(tableResults);

            // Act
            var optionJson = JsonConvert.SerializeObject(options);
            var results = _mockAction.Execute(optionJson);

            // Assert
            Assert.That(results.Data.Rows.Count == 4);
            Assert.That(results.Status == ResultsStatus.Information);
            _mockDatabaseService.Verify(m => m.ExecuteSqlFromFileGeneric(It.IsAny<string>()), Times.Never());
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(5)]
        public void Should_NotModifyData_When_InvalidOptions(int userId)
        {
            // Arrange
            var options = new Options { UserId = userId };
            var tableResults = GetCleanTableResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsUser>(Scripts.GetAdministrators))
                .Returns(tableResults);

            // Act
            var optionJson = JsonConvert.SerializeObject(options);
            var results = _mockAction.Execute(optionJson);

            // Assert
            Assert.That(results.Data.Rows.Count == 4);
            Assert.That(results.Status == ResultsStatus.Error);
            _mockDatabaseService.Verify(m => m.ExecuteSqlFromFileGeneric(It.IsAny<string>()), Times.Never());
        }

        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void Should_ModifyData_When_ValidOptions(int userId)
        {
            // Arrange
            var options = new Options { UserId = userId };
            var tableResults = GetCleanTableResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsUser>(Scripts.GetAdministrators))
                .Returns(tableResults);

            _mockDatabaseService.Invocations.Clear();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFileGeneric(Scripts.ResetAndEnableUser, It.IsAny<object>()))
                .Returns(It.IsAny<IEnumerable<Dictionary<string, object>>>());

            // Act
            var optionJson = JsonConvert.SerializeObject(options);
            var results = _mockAction.Execute(optionJson);

            // Assert
            Assert.That(results.Data.Rows.Count == 4);
            Assert.That(results.Status == ResultsStatus.Good);
            _mockDatabaseService.Verify(m => m.ExecuteSqlFromFileGeneric(It.IsAny<string>(), It.IsAny<object>()), Times.Once());
        }

        private List<CmsUser> GetCleanTableResults()
        {
            return new List<CmsUser>
            {
                new CmsUser
                {
                    UserID = 1,
                    Password = "",
                    Enabled = true
                },
                new CmsUser
                {
                    UserID = 2,
                    Password = "",
                    Enabled = false
                },
                new CmsUser
                {
                    UserID = 3,
                    Password = "testpassword2",
                    Enabled = true
                },
                new CmsUser
                {
                    UserID = 4,
                    Password = "testpassword2",
                    Enabled = false
                }
            };
        }
    }
}