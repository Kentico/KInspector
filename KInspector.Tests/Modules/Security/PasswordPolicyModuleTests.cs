using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kentico.KInspector.Modules;
using System.Collections.Generic;
using System.Linq;
using Kentico.KInspector.Core;
using Moq;
using System.Data;

namespace Kentico.KInspector.Tests.ModuleTests.Security
{
    [TestClass]
    public class PasswordPolicyModuleTests
    {
        [TestMethod]
        public void Should_BeInSecurityCategory_When_ReturningModuleMetadata()
        {
            // arrange...
            PasswordPolicyModule mod = new PasswordPolicyModule();

            // act...
            var meta = mod.GetModuleMetadata();

            // assert...
            Assert.IsTrue(meta.Category.Equals("Security"));
        }

        [TestMethod]
        public void Should_HaveCorrectSupportedVersions_When_ReturningModuleMetadata()
        {
            // arrange...
            List<Version> expectedVersions = new List<Version> { new Version("7.0"), new Version("8.0"), new Version("8.1"), new Version("8.2"), new Version("9.0") };
            PasswordPolicyModule mod = new PasswordPolicyModule();

            // act...
            var meta = mod.GetModuleMetadata();

            // assert...
            Assert.IsTrue((meta.SupportedVersions.Except(expectedVersions).ToList()).Count.Equals(0));
        }

        [TestMethod]
        public void Should_PopulatedName_When_ReturningModuleMetadata()
        {
            // arrange...
            PasswordPolicyModule mod = new PasswordPolicyModule();

            // act...
            var meta = mod.GetModuleMetadata();

            // assert...
            Assert.IsFalse(string.IsNullOrEmpty(meta.Name));
        }

        [TestMethod]
        public void Should_PopulateComments_When_ReturningModuleMetadata()
        {
            // arrange...
            PasswordPolicyModule mod = new PasswordPolicyModule();

            // act...
            var meta = mod.GetModuleMetadata();

            // assert...
            Assert.IsFalse(string.IsNullOrEmpty(meta.Comment));
        }

        [TestMethod]
        public void Should_HaveStatusGood_When_PasswordPolicyDataIsGood()
        {
            // arrange...
            // Mocks...
            var mockDbs = Mock.Of<IDatabaseService>();
            Mock.Get(mockDbs).Setup(_ => _.ExecuteAndGetTableFromFile(It.IsAny<string>())).Returns(this.MakeData(true,3,0));
            var mockInstanceInfo = new Mock<IInstanceInfo>(MockBehavior.Strict);
            mockInstanceInfo.Setup(_ => _.DBService).Returns(mockDbs);

            // Real Module under test...
            PasswordPolicyModule mod = new PasswordPolicyModule();

            // act...
            var result = mod.GetResults(mockInstanceInfo.Object);

            // assert...
            StringAssert.Equals(result.ResultComment, "Password settings look good.");
            Assert.AreEqual(Status.Good, result.Status);
            mockInstanceInfo.VerifyAll();
            Mock.Get(mockDbs).VerifyAll();
        }

        [TestMethod]
        public void Should_HaveStatusError_When_PasswordFormatIsNotCorrect()
        {
            // arrange...
            // Mocks...
            var mockDbs = Mock.Of<IDatabaseService>();
            Mock.Get(mockDbs).Setup(_ => _.ExecuteAndGetTableFromFile(It.IsAny<string>())).Returns(this.MakeData(false, 2,0));
            var mockInstanceInfo = new Mock<IInstanceInfo>(MockBehavior.Strict);
            mockInstanceInfo.Setup(_ => _.DBService).Returns(mockDbs);

            // Real Module under test...
            PasswordPolicyModule mod = new PasswordPolicyModule();

            // act...
            var result = mod.GetResults(mockInstanceInfo.Object);

            // assert...
            StringAssert.Equals(result.ResultComment, "The CMSPasswordFormat should be set to 'SHA2SALT'.");
            Assert.AreEqual(Status.Error, result.Status);
            mockInstanceInfo.VerifyAll();
            Mock.Get(mockDbs).VerifyAll();
        }

        [TestMethod]
        public void Should_HaveStatusError_When_NoRecordsAreRetrieved()
        {
            // arrange...
            // Mocks...
            var mockDbs = Mock.Of<IDatabaseService>();
            Mock.Get(mockDbs).Setup(_ => _.ExecuteAndGetTableFromFile(It.IsAny<string>())).Returns(this.MakeEmptyTable());
            var mockInstanceInfo = new Mock<IInstanceInfo>(MockBehavior.Strict);
            mockInstanceInfo.Setup(_ => _.DBService).Returns(mockDbs);

            // Real Module under test...
            PasswordPolicyModule mod = new PasswordPolicyModule();

            // act...
            var result = mod.GetResults(mockInstanceInfo.Object);

            // assert...
            StringAssert.Equals(result.ResultComment, "Failed to check settings as expected.");
            Assert.AreEqual(Status.Error, result.Status);
            mockInstanceInfo.VerifyAll();
            Mock.Get(mockDbs).VerifyAll();
        }

        [TestMethod]
        public void Should_HaveStatusWarning_When_PasswordPolicyIsFalseForAnySite()
        {
            // arrange...
            // Mocks...
            var mockDbs = Mock.Of<IDatabaseService>();
            Mock.Get(mockDbs).Setup(_ => _.ExecuteAndGetTableFromFile(It.IsAny<string>())).Returns(this.MakeData(true, 2,1));
            var mockInstanceInfo = new Mock<IInstanceInfo>(MockBehavior.Strict);
            mockInstanceInfo.Setup(_ => _.DBService).Returns(mockDbs);

            // Real Module under test...
            PasswordPolicyModule mod = new PasswordPolicyModule();

            // act...
            var result = mod.GetResults(mockInstanceInfo.Object);

            // assert...
            StringAssert.Equals(result.ResultComment, "It is recommended that you have CMSUsePasswordPolicy set to 'True'.");
            Assert.AreEqual(Status.Warning, result.Status);
            mockInstanceInfo.VerifyAll();
            Mock.Get(mockDbs).VerifyAll();
        }

        /// <summary>
        /// Utility method to assist in the creation of 'mock' data for the Unit Tests
        /// </summary>
        /// <param name="hasGoodPasswordFormat">Determines if the mock data table will contain a correct (true) or faulty (false) record for the CMS Password Format </param>
        /// <param name="goodPwdPolicyRowCount">Determines how many correct mock records will be created within the mock table.</param>
        /// <param name="badPwdPolicyRowCount">Determines how many incorrect mock records wil be created within the mock table.</param>
        /// <returns></returns>
        private DataTable MakeData(bool hasGoodPasswordFormat = true, int goodPwdPolicyRowCount = 2, int badPwdPolicyRowCount = 0)
        {
            DataTable tbl = this.MakeEmptyTable();
            DataRow newRow = tbl.NewRow();
            // add a row for password format
            newRow["SiteDisplayName"] = "N/A"; ;
            newRow["KeyName"] = "CMSPasswordFormat";
            newRow["KeyValue"] = hasGoodPasswordFormat ? "SHA2SALT" : "BAD PWD FORMAT";
            tbl.Rows.Add(newRow);
            for (int i = 0; i < goodPwdPolicyRowCount; i++)
            {
                // add a row for password policy
                newRow = tbl.NewRow();
                newRow["SiteDisplayName"] = $"Good Site Name {i}";
                newRow["KeyName"] = "CMSUsePasswordPolicy";
                newRow["KeyValue"] = "True";
                tbl.Rows.Add(newRow);                
            }
            for (int j = 0; j < badPwdPolicyRowCount; j++)
            {
                // add a row for password policy
                newRow = tbl.NewRow();
                newRow["SiteDisplayName"] = $"Bad Site Name {j}";
                newRow["KeyName"] = "CMSUsePasswordPolicy";
                newRow["KeyValue"] = "False";
                tbl.Rows.Add(newRow);                
            }            // return the table
            return tbl;
        }

        /// <summary>
        /// This is the 'mock' table that represents the data the PaswordPolicyModule expects as per the SQL defined 
        /// in its corresponding script, Scripts > PasswordPolicy.sql
        /// </summary>
        /// <returns>An empty Data Table with the expected columns of the 'real' result set.</returns>
        private DataTable MakeEmptyTable()
        {
            DataTable tbl = new DataTable();
            DataColumn columnSpec = null;
            columnSpec = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "SiteDisplayName"
            };
            tbl.Columns.Add(columnSpec);
            columnSpec = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "KeyName"
            };
            tbl.Columns.Add(columnSpec);
            columnSpec = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "KeyValue"
            };
            tbl.Columns.Add(columnSpec);
            return tbl;
        }
    }
}
