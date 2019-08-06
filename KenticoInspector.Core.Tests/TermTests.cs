using KenticoInspector.Core.Models;
using KenticoInspector.Core.Tokens;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace KenticoInspector.Core.Tests
{
    [TestFixture]
    public class TermTests
    {
        public TermTests()
        {
            TokenExpressionResolver.RegisterTokenExpressions(typeof(Term).Assembly);
        }

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
        public void Should_Resolve(string term, string result, params object[] tokenValues)
        {
            TestResolve(term, AsDynamic(tokenValues), result);
        }

        [TestCase("The version is <versiontoken|12.0.4:supported>", "The version is supported", "12.0.4")]
        [TestCase("The version is <?versiontoken=12.0.4:supported>", "The version is supported", "12.0.4")]
        public void Should_Resolve_WithVersion(string term, string result, string version) => Should_Resolve(term, result, new Version(version));

        [TestCaseSource(typeof(TokenExpressionTestCases), nameof(TokenExpressionTestCases.WithSimplePluralization))]
        [TestCaseSource(typeof(TokenExpressionTestCases), nameof(TokenExpressionTestCases.WithTokenOnly))]
        [TestCaseSource(typeof(TokenExpressionTestCases), nameof(TokenExpressionTestCases.WithTokenAndDefaultValue))]
        [TestCaseSource(typeof(TokenExpressionTestCases), nameof(TokenExpressionTestCases.WithOneGreaterThanCase))]
        [TestCaseSource(typeof(TokenExpressionTestCases), nameof(TokenExpressionTestCases.WithOneGreaterThanCaseAndDefaultValue))]
        [TestCaseSource(typeof(TokenExpressionTestCases), nameof(TokenExpressionTestCases.WithOneGreaterThanCaseAndIntegerCaseAndDefaultValue))]
        [TestCaseSource(typeof(TokenExpressionTestCases), nameof(TokenExpressionTestCases.WithOneIntegerCase))]
        [TestCaseSource(typeof(TokenExpressionTestCases), nameof(TokenExpressionTestCases.WithOneIntegerCaseAndDefaultValue))]
        [TestCaseSource(typeof(TokenExpressionTestCases), nameof(TokenExpressionTestCases.WithOneLessThanCase))]
        [TestCaseSource(typeof(TokenExpressionTestCases), nameof(TokenExpressionTestCases.WithOneLessThanCaseAndDefaultValue))]
        [TestCaseSource(typeof(TokenExpressionTestCases), nameof(TokenExpressionTestCases.WithOneLessThanCaseAndDefaultValue))]
        [TestCaseSource(typeof(TokenExpressionTestCases), nameof(TokenExpressionTestCases.WithOneStringCase))]
        [TestCaseSource(typeof(TokenExpressionTestCases), nameof(TokenExpressionTestCases.WithOneStringCaseAndDefaultValue))]
        [TestCaseSource(typeof(TokenExpressionTestCases), nameof(TokenExpressionTestCases.WithTwoIntegerCasesAndDefaultValue))]
        public string Should_ResolveExpression_When_ExpressionIsValid(string termString, object tokenValues)
        {
            Term term = termString;
            var result = term.With(tokenValues);

            return result.ToString();
        }

        // Empty
        [TestCase("This is wrong: <>", typeof(ArgumentException), "value")]
        // Empty
        [TestCase("This is wrong: <?>", typeof(ArgumentException), "value")]
        // Token with colon
        [TestCase("This is <stringtoken:a failure>", typeof(FormatException), "value")]
        // Case with multiple equals
        [TestCase("This is <?stringtoken|stringtoken=value=failure:wrong|a success>", typeof(ArgumentException), "value")]
        public void Should_ThrowException(string term, Type exceptionType, params object[] tokenValues)
        {
            TestThrowException(term, AsDynamic(tokenValues), exceptionType);
        }

        public void TestResolve(Term term, object tokenValues, string result)
        {
            // Act
            string resolvedTerm = term.With(tokenValues).ToString();

            // Assert
            Assert.That(resolvedTerm, Is.EqualTo(result));
        }

        public void TestThrowException(Term term, object tokenValues, Type exceptionType)
        {
            // Act
            string resolvedTermMethod() => term.With(tokenValues).ToString();

            // Assert
            Assert.That(resolvedTermMethod, Throws.TypeOf(exceptionType));
        }

        private static dynamic AsDynamic(object[] tokenValues)
        {
            var dictionary = new Dictionary<string, object>();
            var increment = 1;
            var appendIncrement = tokenValues.Length > 1;

            foreach (var tokenValue in tokenValues)
            {
                var key = $"{tokenValue.GetType().Name}token".ToLower();

                if (appendIncrement)
                {
                    key += increment++;
                }

                dictionary.Add(key, tokenValue);
            }

            return dictionary;
        }
    }

    public class TokenExpressionTestCases
    {
        // TODO: Add WithLessThanCaseAndDefault
        // TODO: Add WithLessThanCaseAndIntegerCaseAndDefault
        // TODO: Add boolean test case to all data sets
        // TODO: Add with special case values (has spaces, has colons, has urls)
        // TODO: Add WithMultipleTokens

        public static IEnumerable<TestCaseData> WithOneGreaterThanCase
        {
            get
            {
                yield return new TestCaseData("<token|>5:5+>", new { token = default(string) }).Returns("");
                yield return new TestCaseData("<token|>5:5+>", new { token = 0 }).Returns("");
                yield return new TestCaseData("<token|>5:5+>", new { token = 1 }).Returns("");
                yield return new TestCaseData("<token|>5:5+>", new { token = 1.5 }).Returns("");
                yield return new TestCaseData("<token|>5:5+>", new { token = 5 }).Returns("");
                yield return new TestCaseData("<token|>5:5+>", new { token = 6 }).Returns("5+");
                yield return new TestCaseData("<token|>5:5+>", new { token = "some text" }).Returns("");
                yield return new TestCaseData("<token|>5:5+>", new { }).Returns("");
            }
        }

        public static IEnumerable<TestCaseData> WithOneGreaterThanCaseAndDefaultValue
        {
            get
            {
                yield return new TestCaseData("<token|>5:5+|resolved default>", new { token = default(string) }).Returns("");
                yield return new TestCaseData("<token|>5:5+|resolved default>", new { token = 0 }).Returns("resolved default");
                yield return new TestCaseData("<token|>5:5+|resolved default>", new { token = 1 }).Returns("resolved default");
                yield return new TestCaseData("<token|>5:5+|resolved default>", new { token = 1.5 }).Returns("resolved default");
                yield return new TestCaseData("<token|>5:5+|resolved default>", new { token = 5 }).Returns("resolved default");
                yield return new TestCaseData("<token|>5:5+|resolved default>", new { token = 6 }).Returns("5+");
                yield return new TestCaseData("<token|>5:5+|resolved default>", new { token = "some text" }).Returns("resolved default");
                yield return new TestCaseData("<token|>5:5+|resolved default>", new { }).Returns("");
            }
        }

        public static IEnumerable<TestCaseData> WithOneGreaterThanCaseAndIntegerCaseAndDefaultValue
        {
            get
            {
                yield return new TestCaseData("<token|>5:many|0:none|some>", new { token = default(string) }).Returns("");
                yield return new TestCaseData("<token|>5:many|0:none|some>", new { token = 0 }).Returns("none");
                yield return new TestCaseData("<token|>5:many|0:none|some>", new { token = 1 }).Returns("some");
                yield return new TestCaseData("<token|>5:many|0:none|some>", new { token = 1.5 }).Returns("some");
                yield return new TestCaseData("<token|>5:many|0:none|some>", new { token = 5 }).Returns("some");
                yield return new TestCaseData("<token|>5:many|0:none|some>", new { token = 6 }).Returns("many");
                yield return new TestCaseData("<token|>5:many|0:none|some>", new { token = "some text" }).Returns("some");
                yield return new TestCaseData("<token|>5:many|0:none|some>", new { }).Returns("");
            }
        }

        public static IEnumerable<TestCaseData> WithOneIntegerCase
        {
            get
            {
                yield return new TestCaseData("<token|0:none>", new { token = default(string) }).Returns("");
                yield return new TestCaseData("<token|0:none>", new { token = 0 }).Returns("none");
                yield return new TestCaseData("<token|0:none>", new { token = 1 }).Returns("");
                yield return new TestCaseData("<token|0:none>", new { token = 1.5 }).Returns("");
                yield return new TestCaseData("<token|0:none>", new { token = 5 }).Returns("");
                yield return new TestCaseData("<token|0:none>", new { token = "some text" }).Returns("");
                yield return new TestCaseData("<token|0:none>", new { }).Returns("");
            }
        }

        public static IEnumerable<TestCaseData> WithOneIntegerCaseAndDefaultValue
        {
            get
            {
                yield return new TestCaseData("<token|0:none|at least one>", new { token = default(string) }).Returns("");
                yield return new TestCaseData("<token|0:none|at least one>", new { token = 0 }).Returns("none");
                yield return new TestCaseData("<token|0:none|at least one>", new { token = 1 }).Returns("at least one");
                yield return new TestCaseData("<token|0:none|at least one>", new { token = 1.5 }).Returns("at least one");
                yield return new TestCaseData("<token|0:none|at least one>", new { token = 5 }).Returns("at least one");
                yield return new TestCaseData("<token|0:none|at least one>", new { token = "some text" }).Returns("at least one");
                yield return new TestCaseData("<token|0:none|at least one>", new { }).Returns("");
            }
        }

        public static IEnumerable<TestCaseData> WithOneLessThanCase
        {
            get
            {
                var tokenExpression = "<token|<5:less than five>";
                var returnsEmptyResult = string.Empty;
                var returnsCase1Result = "less than five";

                yield return GetTestCaseData(tokenExpression, TokenValueOptions.TokenIsNull, returnsEmptyResult);
                yield return GetTestCaseData(tokenExpression, TokenValueOptions.TokenIs0, returnsCase1Result);
                //yield return GetTestCaseData(nameof(WithOneLessThanCase), token, TokenValueOptions.TokenIs0, matchCase1Result);
                //yield return GetTestCaseData(nameof(WithOneLessThanCase), token, new { token = 1 }, matchCase1Result);
                //yield return GetTestCaseData(nameof(WithOneLessThanCase), token, new { token = 1.5 }, matchCase1Result);
                //yield return GetTestCaseData(nameof(WithOneLessThanCase), token, new { token = 5 }, noMatch);
                //yield return GetTestCaseData(nameof(WithOneLessThanCase), token, new { token = true }, noMatch);
                //yield return GetTestCaseData(nameof(WithOneLessThanCase), token, new { token = false }, noMatch);
                //yield return GetTestCaseData(nameof(WithOneLessThanCase), token, new { token = "some text" }, noMatch);
                //yield return GetTestCaseData(nameof(WithOneLessThanCase), token, new { }, noMatch);
            }
        }

        private static TestCaseData GetTestCaseData(string tokenExpression, object tokenValues, string resolved, [CallerMemberName] string category = "")
        {

            return new TestCaseData(tokenExpression, tokenValues)
                .Returns(resolved)
                .SetName($"Token Expression: \"{tokenExpression}\" with \"{tokenValues}\" resolves to \"{resolved}\"")
                .SetCategory(category);
        }

        public static IEnumerable<TestCaseData> WithOneLessThanCaseAndDefaultValue
        {
            get
            {
                yield return new TestCaseData("<token|<5:0-4|had a value>", TokenValueOptions.TokenIsNull).Returns("");
                yield return new TestCaseData("<token|<5:0-4|had a value>", TokenValueOptions.TokenIs0).Returns("0-4");
                yield return new TestCaseData("<token|<5:0-4|had a value>", new { token = 1 }).Returns("0-4");
                yield return new TestCaseData("<token|<5:0-4|had a value>", new { token = 1.5 }).Returns("0-4");
                yield return new TestCaseData("<token|<5:0-4|had a value>", new { token = 5 }).Returns("had a value");
                yield return new TestCaseData("<token|<5:0-4|had a value>", new { token = true }).Returns("had a value");
                yield return new TestCaseData("<token|<5:0-4|had a value>", new { token = false }).Returns("had a value");
                yield return new TestCaseData("<token|<5:0-4|had a value>", new { token = "some text" }).Returns("had a value");
                yield return new TestCaseData("<token|<5:0-4|had a value>", new { }).Returns("");
            }
        }

        public static IEnumerable<TestCaseData> WithOneStringCase
        {
            get
            {
                yield return new TestCaseData("<token|some text:resolved>", new { token = default(string) }).Returns("");
                yield return new TestCaseData("<token|some text:resolved>", new { token = 0 }).Returns("");
                yield return new TestCaseData("<token|some text:resolved>", new { token = 1 }).Returns("");
                yield return new TestCaseData("<token|some text:resolved>", new { token = 1.5 }).Returns("");
                yield return new TestCaseData("<token|some text:resolved>", new { token = 5 }).Returns("");
                yield return new TestCaseData("<token|some text:resolved>", new { token = "some text" }).Returns("resolved");
                yield return new TestCaseData("<token|some text:resolved>", new { }).Returns("");
            }
        }

        public static IEnumerable<TestCaseData> WithOneStringCaseAndDefaultValue
        {
            get
            {
                yield return new TestCaseData("<token|some text:resolved|resolved default>", new { token = default(string) }).Returns("");
                yield return new TestCaseData("<token|some text:resolved|resolved default>", new { token = 0 }).Returns("resolved default");
                yield return new TestCaseData("<token|some text:resolved|resolved default>", new { token = 1 }).Returns("resolved default");
                yield return new TestCaseData("<token|some text:resolved|resolved default>", new { token = 1.5 }).Returns("resolved default");
                yield return new TestCaseData("<token|some text:resolved|resolved default>", new { token = 5 }).Returns("resolved default");
                yield return new TestCaseData("<token|some text:resolved|resolved default>", new { token = "some text" }).Returns("resolved");
                yield return new TestCaseData("<token|some text:resolved|resolved default>", new { }).Returns("");
            }
        }

        public static IEnumerable<TestCaseData> WithSimplePluralization
        {
            get
            {
                yield return new TestCaseData("<token|issue|issues>", new { token = default(string) }).Returns("");
                yield return new TestCaseData("<token|issue|issues>", new { token = 0 }).Returns("issues");
                yield return new TestCaseData("<token|issue|issues>", new { token = 1 }).Returns("issue");
                yield return new TestCaseData("<token|issue|issues>", new { token = 1.5 }).Returns("issues");
                yield return new TestCaseData("<token|issue|issues>", new { token = 5 }).Returns("issues");
                yield return new TestCaseData("<token|issue|issues>", new { token = "some text" }).Returns("issues");
                yield return new TestCaseData("<token|issue|issues>", new { }).Returns("");
            }
        }

        public static IEnumerable<TestCaseData> WithTokenAndDefaultValue
        {
            get
            {
                yield return new TestCaseData("<token|found>", new { token = default(string) }).Returns("");
                yield return new TestCaseData("<token|found>", new { token = 0 }).Returns("found");
                yield return new TestCaseData("<token|found>", new { token = 1 }).Returns("found");
                yield return new TestCaseData("<token|found>", new { token = 1.5 }).Returns("found");
                yield return new TestCaseData("<token|found>", new { token = 5 }).Returns("found");
                yield return new TestCaseData("<token|found>", new { token = "some text" }).Returns("found");
                yield return new TestCaseData("<token|found>", new { token = true }).Returns("found");
                yield return new TestCaseData("<token|found>", new { }).Returns("");

            }
        }

        public static IEnumerable<TestCaseData> WithTokenOnly
        {
            get
            {
                yield return new TestCaseData("<token>", new { token = default(string) }).Returns("");
                yield return new TestCaseData("<token>", new { token = 0 }).Returns("0");
                yield return new TestCaseData("<token>", new { token = 1 }).Returns("1");
                yield return new TestCaseData("<token>", new { token = 1.5 }).Returns("1.5");
                yield return new TestCaseData("<token>", new { token = 5 }).Returns("5");
                yield return new TestCaseData("<token>", new { token = "some text" }).Returns("some text");
                yield return new TestCaseData("<token>", new { }).Returns("");
            }
        }

        public static IEnumerable<TestCaseData> WithTwoIntegerCasesAndDefaultValue
        {
            get
            {
                yield return new TestCaseData("<token|0:none|1:one|some>", new { token = default(string) }).Returns("");
                yield return new TestCaseData("<token|0:none|1:one|some>", new { token = 0 }).Returns("none");
                yield return new TestCaseData("<token|0:none|1:one|some>", new { token = 1 }).Returns("one");
                yield return new TestCaseData("<token|0:none|1:one|some>", new { token = 1.5 }).Returns("some");
                yield return new TestCaseData("<token|0:none|1:one|some>", new { token = 5 }).Returns("some");
                yield return new TestCaseData("<token|0:none|1:one|some>", new { token = "some text" }).Returns("some");
                yield return new TestCaseData("<token|0:none|1:one|some>", new { }).Returns("");
            }
        }
    }

    public class TokenValueOptions
    {
        public static object TokenIsNull => new { token = default(string) };
        public static object TokenIs0 => new { token = 0 };
    }
}