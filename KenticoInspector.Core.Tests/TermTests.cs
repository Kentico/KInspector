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
        // Basic simple
        [TestCase("<int32token> car", "1 car", 1)]
        [TestCase("<int32token> cars", "5 cars", 5)]
        // Explicit int case
        [TestCase("<int32token|5:Many> cars", "Many cars", 5)]
        // Implicit singular
        [TestCase("<int32token|One|Many> car", "One car", 1)]
        // Implicit plural
        [TestCase("<int32token|One|Many> cars", "Many cars", 5)]
        [TestCase("<int32token> <int32token|car|cars> found", "5 cars found", 5)]
        // Conditional cases
        [TestCase("<int32token|>5:More than five|Many> cars", "More than five cars", 6)]
        [TestCase("<int32token|<5:Less than five|Many> cars", "Many cars", 6)]
        // Explicit string case
        [TestCase("<stringtoken|value:The> cars", "The cars", "value")]
        // Default string case
        [TestCase("<stringtoken|value:wrong|defaultvalue:The red> car", "The red car", "defaultvalue")]
        // String case with colon
        [TestCase("<stringtoken|value:The: red> car", "The: red car", "value")]
        // Default string case with space
        [TestCase("There is <stringtoken1|a truck|a car> <stringtoken2>.", "There is a car here.", "blue", "here")]
        // Empty with one space when no match
        [TestCase("The <stringtoken|value:red> car", "The car", "blue")]
        [TestCase("The <wrongtoken> car", "The car", "red")]
        // Multiple expressions
        [TestCase("<stringtoken1> and <stringtoken2|nothing:an empty|a burning> truck", "No cars and an empty truck", "No cars", "nothing")]
        [TestCase("<stringtoken1> and <stringtoken2|truck:a truck|a van>.", "No cars and a truck.", "No cars", "truck")]
        // URL
        [TestCase("<stringtoken>: https://docs.kentico.com", "No cars: https://docs.kentico.com", "No cars")]
        // Basic advanced
        [TestCase("<?int32token> car", "1 car", 1)]
        [TestCase("<?int32token> cars", "5 cars", 5)]
        // Explicit advanced case
        [TestCase("<?int32token=5:Many> cars", "Many cars", 5)]
        // Implicit singular
        [TestCase("<?int32token:One|Many> car", "One car", 1)]
        // Implicit plural
        [TestCase("<?int32token:One|Many> cars", "Many cars", 5)]
        // Conditional cases
        [TestCase("The <?int32token>5:red|wrongtoken=car:blue> vehicle is going very fast", "The red vehicle is going very fast", 6)]
        [TestCase("The <?int32token<5:red|blue> vehicle is going very fast", "The blue vehicle is going very fast", 6)]
        // String case with colon
        [TestCase("<?stringtoken=value:The: red> car", "The: red car", "value")]
        // Multiple cases, one match
        [TestCase("The <?stringtoken1=truck:red|stringtoken2=car:blue> vehicle is going very fast", "The red vehicle is going very fast", "truck", "van")]
        // Multiple cases, default match
        [TestCase("The <?stringtoken1=truck:red|stringtoken2=car:blue|light green> vehicle is going very fast", "The light green vehicle is going very fast", "moped", "van")]
        // Multiple cases, first match
        [TestCase("The <?stringtoken1=truck:red|stringtoken1=car:blue> vehicle is going very fast", "The red vehicle is going very fast", "truck", "car")]
        // Multiple cases, second token match
        [TestCase("The <?stringtoken2=car:red|wrongtoken=car:blue> vehicle is going very fast", "The red vehicle is going very fast", "truck", "car")]
        // Empty with one space when no match
        [TestCase("The <?wrongtoken=car:Blue> vehicle is going very fast", "The vehicle is going very fast", "truck", "car")]
        // URL
        [TestCase("<?stringtoken>: https://docs.kentico.com", "No cars: https://docs.kentico.com", "No cars")]
        public void ShouldResolve(string term, string result, params object[] tokenValues)
        {
            TestValidResult(term, AsDynamic(tokenValues), result);
        }

        [Test]
        [TestCase("The version is <versiontoken|12.0.4:supported>", "The version is supported", "12.0.4")]
        [TestCase("The version is <?versiontoken=12.0.4:supported>", "The version is supported", "12.0.4")]
        public void ShouldResolveWithVersion(string term, string result, string version) => ShouldResolve(term, result, new Version(version));

        [Test]
        // Empty
        //[TestCase("This is wrong: <>", typeof(ArgumentException), "value")]
        // Case with colon without space
        [TestCase("This is <stringtoken|value:failure:wrong|a success>", typeof(ArgumentException), "value")]
        // Empty
        [TestCase("This is wrong: <?>", typeof(ArgumentException), "value")]
        // Case with multiple equals
        [TestCase("This is <?stringtoken|stringtoken=value=failure:wrong|a success>", typeof(ArgumentException), "value")]
        // Case with colon without space
        [TestCase("This is <?stringtoken|stringtoken=value:failure:wrong|a success>", typeof(ArgumentException), "value")]
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