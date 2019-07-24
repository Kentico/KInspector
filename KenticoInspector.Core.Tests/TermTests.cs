using System;
using KenticoInspector.Core.Models;
using NUnit.Framework;

namespace KenticoInspector.Core.Tests
{
    [TestFixture]
    public class TermTests
    {
        public TermTests()
        {
        }

        [Test]
        public void ShouldResolveWithInt()
        {
            TestValidResult(
                "<intToken> results",
                new { intToken = 5 },
                "5 results"
                );
        }

        [Test]
        public void ShouldResolveWithIntSingleCase()
        {
            TestValidResult(
                "<intToken|5:Many> results",
                new { intToken = 5 },
                "Many results"
                );
        }

        [Test]
        public void ShouldResolveWithIntPluralizationSingle()
        {
            TestValidResult(
                "<intToken|One|Many> results",
                new { intToken = 1 },
                "One results"
                );
        }

        [Test]
        public void ShouldResolveWithIntPluralizationPlural()
        {
            TestValidResult(
                "<intToken|One|Many> results",
                new { intToken = 5 },
                "Many results"
                );
        }

        [Test]
        public void ShouldResolveWithStringSingleCase()
        {
            TestValidResult(
                "<stringToken|value:Correct> result",
                new { stringToken = "value" },
                "Correct result"
                );
        }

        [Test]
        public void ShouldResolveWithStringSingleCaseDefault()
        {
            TestValidResult(
                "<stringToken|value:wrong|defaultvalue:Correct> result",
                new { stringToken = "defaultvalue" },
                "Correct result"
                );
        }

        [Test]
        public void ShouldResolveWithStringSingleCaseDefaultWithSpaces()
        {
            TestValidResult(
                "<stringToken|value:wrong|defaultvalue:Really correct> result",
                new { stringToken = "defaultvalue" },
                "Really correct result"
                );
        }

        [Test]
        public void ShouldResolveWithStringSingleCaseNoDefault()
        {
            TestValidResult(
                "Is <stringToken|value:not >true",
                new { stringToken = "defaultvalue" },
                "Is true"
                );
        }

        [Test]
        public void ShouldResolveWithMultipleStringsWithSpaces()
        {
            TestValidResult(
                "<stringToken> and <stringToken2|nothing:nothing wrong|something wrong>",
                new { stringToken = "No results", stringToken2 = "nothing" },
                "No results and nothing wrong"
                );
        }

        [Test]
        public void ShouldResolveWithVersion()
        {
            TestValidResult(
                "The version is <version|12.0.4:supported>",
                new { version = new Version(12, 0, 4) },
                "The version is supported"
                );
        }

        [Test]
        public void ShouldNotResolveInvalidToken()
        {
            TestInvalidThrows<ArgumentException>(
                "This is wrong: <>",
                new { stringToken = "value" }
                );
        }

        [Test]
        public void ShouldNotResolveInvalidImpliedCase()
        {
            TestInvalidThrows<FormatException>(
                "This is <notInt|a failure|a success>",
                new { notInt = "string" }
                );
        }

        [Test]
        public void ShouldNotResolveInvalidCase()
        {
            TestInvalidThrows<FormatException>(
                "This is <stringToken|string:failure:wrong|a success>",
                new { stringToken = "value" }
                );
        }

        public void TestValidResult(Term term, object tokenValues, string result)
        {
            // Act
            var resolvedTerm = term.With(tokenValues);

            // Assert
            Assert.That(resolvedTerm, Is.EqualTo(result));
        }

        public void TestInvalidThrows<TException>(Term term, object tokenValues) where TException : Exception
        {
            // Assert
            Assert.That(() => term.With(tokenValues), Throws.TypeOf<TException>());
        }
    }
}