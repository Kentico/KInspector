using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using KenticoInspector.Core.Models;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models;

namespace KenticoInspector.Reports.SecuritySettingsAnalysis.Analyzers
{
    public abstract class AbstractAnalyzers<TData, TResult> where TResult : class
    {
        private IEnumerable<MethodInfo> analyzers;

        protected Terms ReportTerms { get; }

        protected IEnumerable<MethodInfo> Analyzers => analyzers ?? (analyzers = GetType()
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(method => method.ReturnType == typeof(TResult)));

        public AbstractAnalyzers(Terms reportTerms)
        {
            ReportTerms = reportTerms;
        }

        public TResult GetAnalysis(object setting, string name)
        {
            TResult result = null;

            var matchingAnalyzers = Analyzers
                .Where(analyzer => Match(analyzer.Name, name));

            foreach (var analyzer in matchingAnalyzers)
            {
                result = analyzer.Invoke(this, new[] { setting }) as TResult ?? result;
            }

            return result;
        }

        protected virtual bool Match(string analyzerName, string name)
        {
            return analyzerName
                .Equals(name, StringComparison.InvariantCulture);
        }

        protected virtual TResult AnalyzeUsingString(TData setting, string recommendedValue, Term recommendationReason)
            => AnalyzeUsingFunc(
                setting,
                value => value.Equals(recommendedValue, StringComparison.InvariantCultureIgnoreCase),
                recommendedValue,
                recommendationReason
                );

        protected virtual TResult AnalyzeUsingFunc(
            TData setting,
            Func<string, bool> valueIsRecommended,
            string recommendedValue,
            Term recommendationReason
            )
        {
            throw new NotImplementedException();
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
    }
}