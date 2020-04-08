using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using KenticoInspector.Core.Models;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models;

namespace KenticoInspector.Reports.SecuritySettingsAnalysis.Analyzers
{
    public abstract class AbstractAnalyzers<TData, TResult> where TResult : class
    {
        protected Terms ReportTerms { get; }

        public abstract IEnumerable<Expression<Func<TData, TResult>>> Analyzers { get; }

        public AbstractAnalyzers(Terms reportTerms)
        {
            ReportTerms = reportTerms;
        }

        public TResult GetAnalysis(
            Expression<Func<TData, TResult>> analyzer,
            IEnumerable<TData> settings,
            Func<TData, string> getSettingName
            )
        {
            TResult result = null;

            foreach (var setting in settings)
            {
                var expectedSettingName = analyzer.Parameters[0].Name;

                if (Match(expectedSettingName, getSettingName(setting)))
                {
                    result = analyzer.Compile()(setting) as TResult ?? result;
                }
            }

            return result;
        }

        protected virtual bool Match(string analyzerName, string name)
        {
            return analyzerName.Equals(name, StringComparison.InvariantCulture);
        }

        protected virtual TResult AnalyzeUsingExpression(
            TData element,
            Expression<Func<string, bool>> valueIsRecommended,
            string recommendedValue,
            Term recommendationReason
            )
        {
            throw new NotImplementedException();
        }

        protected static bool IsNullOrEquals(string value, string equals)
        {
            return value == null || Equals(value, equals);
        }

        protected static bool Equals(string value, string equals)
        {
            return value != null && value.Equals(equals, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}