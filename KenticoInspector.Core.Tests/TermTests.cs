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
        [TestCase("This is wrong: //", typeof(ArgumentException), "value")]
        [TestCase("This is <stringtoken|string:failure:wrong|a success>", typeof(ArgumentException), "value")]
        [TestCase("This is /stringtoken|string=failure=value:wrong|a success/", typeof(ArgumentException), "value")]
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