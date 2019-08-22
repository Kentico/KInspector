using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xml.Linq;

using KenticoInspector.Core.Models;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models.Data.Results;

namespace KenticoInspector.Reports.SecuritySettingsAnalysis.Analyzers
{
    public class ConnectionStringAnalyzers : AbstractAnalyzers<XElement, WebConfigSettingResult>
    {
        public override IEnumerable<Expression<Func<XElement, WebConfigSettingResult>>> Analyzers
            => new List<Expression<Func<XElement, WebConfigSettingResult>>>
        {
            CMSConnectionString => AnalyzeUsingExpression(
                CMSConnectionString,
                connectionString
                    => !connectionString.Contains("user id=sa;", StringComparison.InvariantCultureIgnoreCase),
                ReportTerms.RecommendedValues.NotSaUser,
                ReportTerms.RecommendationReasons.ConnectionStrings.SaUser
                ),
        };

        public ConnectionStringAnalyzers(Terms reportTerms) : base(reportTerms)
        {
        }

        protected override WebConfigSettingResult AnalyzeUsingExpression(
            XElement connectionString,
            Expression<Func<string, bool>> valueIsRecommended,
            string recommendedValue,
            Term recommendationReason
            )
        {
            string attributeName = valueIsRecommended.Parameters[0].Name;

            string keyValue = connectionString.Attribute(attributeName)?.Value;

            if (valueIsRecommended.Compile()(keyValue)) return null;

            string keyName = connectionString.Attribute("name").Value;

            return new WebConfigSettingResult(connectionString, keyName, keyValue, recommendedValue, recommendationReason);
        }
    }
}