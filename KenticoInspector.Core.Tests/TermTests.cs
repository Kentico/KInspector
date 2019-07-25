using System;
using System.Collections.Generic;
using System.Dynamic;

using KenticoInspector.Core.Models;
using KenticoInspector.Core.Tokens;

using NUnit.Framework;

namespace KenticoInspector.Core.Tests
{
    [TestFixture]
    public class TermTests
    {
        public TermTests()
        {
            TokenProcessor.RegisterTokens(typeof(Term).Assembly);
        }

        [Test]
        [TestCase("<int32token> results", "5 results", 5)]
        [TestCase("<int32token|5:Many> results", "Many results", 5)]
        [TestCase("<int32token|One|Many> results", "One results", 1)]
        [TestCase("<int32token|One|Many> results", "Many results", 5)]
        [TestCase("<stringtoken|value:Correct> results", "Correct results", "value")]
        [TestCase("<stringtoken|value:wrong|defaultvalue:Correct> result", "Correct result", "defaultvalue")]
        [TestCase("<stringtoken|value:wrong|defaultvalue:Really correct> result", "Really correct result", "defaultvalue")]
        [TestCase("Is <stringtoken|value:not >true", "Is true", "defaultvalue")]
        [TestCase("<stringtoken1> and <stringtoken2|nothing:nothing wrong|something wrong>", "No results and nothing wrong", "No results", "nothing")]
        public void ShouldResolve(string term, string result, params object[] tokenValues)
        {
            TestValidResult(
                term,
                AsDynamic(tokenValues),
                result
            );
        }

        [Test]
        public void ShouldResolveWithVersion() => ShouldResolve("The version is <versiontoken|12.0.4:supported>", "The version is supported", new Version(12, 0, 4));

        [Test]
        [TestCase("This is wrong: <>", typeof(ArgumentException), "value")]
        [TestCase("This is <stringtoken|a failure|a success>", typeof(FormatException), "value")]
        [TestCase("This is <stringtoken|string:failure:wrong|a success>", typeof(FormatException), "value")]
        public void ShouldNotResolve(string term, Type exceptionType, params object[] tokenValues)
        {
            TestInvalidThrows(
                term,
                exceptionType,
                AsDynamic(tokenValues)
            );
        }

        private static dynamic AsDynamic(object[] tokenValues)
        {
            var expandoObject = new ExpandoObject();

            var dictionary = (IDictionary<string, object>)expandoObject;

            var i = 1;

            foreach (var tokenValue in tokenValues)
            {
                var key = $"{tokenValue.GetType().Name}token".ToLower();

                if (tokenValues.Length > 1)
                {
                    key += i++;
                }

                dictionary.Add(key, tokenValue);
            }

            return expandoObject;
        }

        public void TestValidResult(Term term, object tokenValues, string result)
        {
            // Act
            var resolvedTerm = term.With(tokenValues);

            // Assert
            Assert.That(resolvedTerm.ToString(), Is.EqualTo(result));
        }

        public void TestInvalidThrows(Term term, Type exceptionType, object tokenValues)
        {
            // Assert
            Assert.That(() => term.With(tokenValues).ToString(), Throws.TypeOf(exceptionType));
        }
    }
}