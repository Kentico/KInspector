using System;
using System.Collections.Generic;

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
            TokenExpressionResolver.RegisterTokenExpressions(typeof(Term).Assembly);
        }

        [Test]
        [TestCase("<int32token> cars", "5 cars", 5)]
        [TestCase("<int32token|5:Many> cars", "Many cars", 5)]
        [TestCase("<int32token|One|Many> car", "One car", 1)]
        [TestCase("<int32token|One|Many> cars", "Many cars", 5)]
        [TestCase("<stringtoken|value:The> cars", "The cars", "value")]
        [TestCase("<stringtoken|value:wrong|defaultvalue:The> car", "The car", "defaultvalue")]
        [TestCase("<stringtoken|value:wrong|defaultvalue:The red> car", "The red car", "defaultvalue")]
        [TestCase("The < stringtoken|value:red >car", "The car", "blue")]
        [TestCase("The <wrongtoken>car", "The car", "red")]
        [TestCase("<stringtoken1> and <stringtoken2|nothing:an empty|a burning> truck", "No cars and an empty truck", "No cars", "nothing")]
        [TestCase("This is <stringtoken|a truck|a car>", "This is a car", "blue")]
        [TestCase("The /stringtoken1=truck:Red|stringtoken2=car:Blue/ vehicle is going very fast", "The Red vehicle is going very fast", "truck", "van")]
        [TestCase("The /stringtoken1=truck:Red|stringtoken2=car:Blue|light green/ vehicle is going very fast", "The light green vehicle is going very fast", "moped", "van")]
        [TestCase("The /stringtoken1=truck:Red|stringtoken1=car:Blue/ vehicle is going very fast", "The Red vehicle is going very fast", "truck", "car")]
        [TestCase("The /stringtoken2=car:Red|wrongtoken=car:Blue/ vehicle is going very fast", "The Red vehicle is going very fast", "truck", "car")]
        [TestCase("The /wrongtoken=car:Blue /vehicle is going very fast", "The vehicle is going very fast", "truck", "car")]
        public void ShouldResolve(string term, string result, params object[] tokenValues)
        {
            TestValidResult(term, AsDynamic(tokenValues), result);
        }

        [Test]
        [TestCase("The version is <versiontoken|12.0.4:supported>", "The version is supported", "12.0.4")]
        [TestCase("The version is /versiontoken=12.0.4:supported/", "The version is supported", "12.0.4")]
        public void ShouldResolveWithVersion(string term, string result, string version) => ShouldResolve(term, result, new Version(version));

        [Test]
        [TestCase("This is wrong: <>", typeof(ArgumentException), "value")]
        [TestCase("This is wrong: //", typeof(ArgumentException), "value")]
        [TestCase("This is <stringtoken|value:failure:wrong|a success>", typeof(ArgumentException), "value")]
        [TestCase("This is /stringtoken|stringtoken=value=failure:wrong|a success/", typeof(ArgumentException), "value")]
        public void ShouldNotResolve(string term, Type exceptionType, params object[] tokenValues)
        {
            TestInvalidThrows(term, AsDynamic(tokenValues), exceptionType);
        }

        private static dynamic AsDynamic(object[] tokenValues)
        {
            var dictionary = new Dictionary<string, object>();
            var increment = 1;
            var appendIncrement = tokenValues.Length > 1;

            foreach (var tokenValue in tokenValues)
            {
                var key = $"{tokenValue.GetType().Name}token".ToLower();

                if (appendIncrement) key += increment++;

                dictionary.Add(key, tokenValue);
            }

            return dictionary;
        }

        public void TestValidResult(Term term, object tokenValues, string result)
        {
            // Act
            string resolvedTerm = term.With(tokenValues).ToString();

            // Assert
            Assert.That(resolvedTerm, Is.EqualTo(result));
        }

        public void TestInvalidThrows(Term term, object tokenValues, Type exceptionType)
        {
            // Act
            string resolvedTermMethod() => term.With(tokenValues).ToString();

            // Assert
            Assert.That(resolvedTermMethod, Throws.TypeOf(exceptionType));
        }
    }
}