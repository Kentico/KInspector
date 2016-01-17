﻿using System;
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
            List<Version> expectedVersions = new List<Version>() { new Version("7.0"), new Version("8.0"), new Version("8.1"), new Version("8.2"), new Version("9.0") };
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
            Mock.Get(mockDbs).Setup(_ => _.ExecuteAndGetTableFromFile(It.IsAny<string>())).Returns(this.MakeDataTableOK());
            var mockInstanceInfo = new Mock<IInstanceInfo>(MockBehavior.Strict);
            mockInstanceInfo.Setup(_ => _.DBService).Returns(mockDbs);

            // Real Module under test...
            PasswordPolicyModule mod = new PasswordPolicyModule();

            // act...
            var result = mod.GetResults(mockInstanceInfo.Object);

            // assert...
            Assert.IsTrue(result.ResultComment.Equals("Password settings look good."));
            Assert.IsTrue(result.Status.Equals(Status.Good));
            mockInstanceInfo.VerifyAll();
            Mock.Get(mockDbs).VerifyAll();
        }

        [TestMethod]
        public void Should_HaveStatusError_When_PasswordFormatIsNotCorrect()
        {
            // arrange...
            // Mocks...
            var mockDbs = Mock.Of<IDatabaseService>();
            Mock.Get(mockDbs).Setup(_ => _.ExecuteAndGetTableFromFile(It.IsAny<string>())).Returns(this.MakeDataTableWithBadPasswordFormat());
            var mockInstanceInfo = new Mock<IInstanceInfo>(MockBehavior.Strict);
            mockInstanceInfo.Setup(_ => _.DBService).Returns(mockDbs);

            // Real Module under test...
            PasswordPolicyModule mod = new PasswordPolicyModule();

            // act...
            var result = mod.GetResults(mockInstanceInfo.Object);

            // assert...
            Assert.IsTrue(result.ResultComment.Equals("The CMSPasswordFormat should be set to 'SHA2SALT'."));
            Assert.IsTrue(result.Status.Equals(Status.Error));
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
            Assert.IsTrue(result.ResultComment.Equals("Failed to check settings as expected."));
            Assert.IsTrue(result.Status.Equals(Status.Error));
            mockInstanceInfo.VerifyAll();
            Mock.Get(mockDbs).VerifyAll();
        }

        [TestMethod]
        public void Should_HaveStatusWarning_When_PasswordPolicyIsFalseForAnySite()
        {
            // arrange...
            // Mocks...
            var mockDbs = Mock.Of<IDatabaseService>();
            Mock.Get(mockDbs).Setup(_ => _.ExecuteAndGetTableFromFile(It.IsAny<string>())).Returns(this.MakeDataTableWithBadPasswordFormat());
            var mockInstanceInfo = new Mock<IInstanceInfo>(MockBehavior.Strict);
            mockInstanceInfo.Setup(_ => _.DBService).Returns(mockDbs);

            // Real Module under test...
            PasswordPolicyModule mod = new PasswordPolicyModule();

            // act...
            var result = mod.GetResults(mockInstanceInfo.Object);

            // assert...
            Assert.IsTrue(result.ResultComment.Equals("The CMSPasswordFormat should be set to 'SHA2SALT'."));
            Assert.IsTrue(result.Status.Equals(Status.Error));
            mockInstanceInfo.VerifyAll();
            Mock.Get(mockDbs).VerifyAll();
        }

        private DataTable MakeDataTableOK()
        {
            DataTable tbl = this.MakeEmptyTable();
            DataRow newRow = tbl.NewRow();
            // add a row for password format
            newRow["SiteDisplayName"] = "NA";
            newRow["KeyName"] = "CMSPasswordFormat";
            newRow["KeyValue"] = "SHA2SALT";
            tbl.Rows.Add(newRow);
            // add a row for password policy
            newRow = tbl.NewRow();
            newRow["SiteDisplayName"] = "K-Site-1";
            newRow["KeyName"] = "CMSUsePasswordPolicy";
            newRow["KeyValue"] = "True";
            tbl.Rows.Add(newRow);
            // return the table
            return tbl;
        }

        private DataTable MakeDataTableWithBadPasswordFormat()
        {
            DataTable tbl = this.MakeEmptyTable();
            DataRow newRow = tbl.NewRow();
            // add a row for password format
            newRow["SiteDisplayName"] = "NA";
            newRow["KeyName"] = "CMSPasswordFormat";
            newRow["KeyValue"] = "BAD FORMAT";
            tbl.Rows.Add(newRow);
            // add a row for password policy
            newRow = tbl.NewRow();
            newRow["SiteDisplayName"] = "K-Site-1";
            newRow["KeyName"] = "CMSUsePasswordPolicy";
            newRow["KeyValue"] = "True";
            tbl.Rows.Add(newRow);
            // return the table
            return tbl;
        }

        private DataTable MakeDataTableWithBadPasswordPolicy()
        {
            DataTable tbl = this.MakeEmptyTable();
            DataRow newRow = tbl.NewRow();
            // add a row for password format
            newRow["SiteDisplayName"] = "NA";
            newRow["KeyName"] = "CMSPasswordFormat";
            newRow["KeyValue"] = "SHA2SALT";
            tbl.Rows.Add(newRow);
            // add a row for password policy
            newRow = tbl.NewRow();
            newRow["SiteDisplayName"] = "K-Site-1";
            newRow["KeyName"] = "CMSUsePasswordPolicy";
            newRow["KeyValue"] = "True";
            tbl.Rows.Add(newRow);
            // add a row for password policy
            newRow = tbl.NewRow();
            newRow["SiteDisplayName"] = "K-Site-1";
            newRow["KeyName"] = "CMSUsePasswordPolicy";
            newRow["KeyValue"] = "False";
            tbl.Rows.Add(newRow);
            // return the table
            return tbl;
        }

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
