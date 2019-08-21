using System;
using System.Collections.Generic;
using System.Linq;
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
            var matchingAnalyzer = Analyzers
                .FirstOrDefault(analyzer => Match(analyzer.Name, name));

            return matchingAnalyzer?.Invoke(this, new[] { setting }) as TResult;
        }

        protected virtual bool Match(string analyzerName, string name)
        {
            return analyzerName
                .Equals(name, StringComparison.InvariantCulture);
        }

        protected TResult AnalyzeUsingString(TData setting, string recommendedValue, Term recommendationReason)
            => AnalyzeUsingFunc(
                setting,
                value => value.Equals(recommendedValue, StringComparison.InvariantCultureIgnoreCase),
                recommendedValue,
                recommendationReason
                );

        protected abstract TResult AnalyzeUsingFunc(
            TData setting,
            Func<string, bool> valueIsRecommended,
            string recommendedValue,
            Term recommendationReason
            );
    }
}