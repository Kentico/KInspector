using KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Data;

namespace KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Results
{
    public class TransformationUsageResult
    {
        public int TemplateId { get; set; }

        public string TemplateCodeName { get; set; }

        public string TemplateDisplayName { get; set; }

        public string WebPart { get; set; }

        public string Property { get; set; }

        public string Transformation { get; set; }

        public string TransformationType { get; set; }

        public TransformationUsageResult(PageTemplate pageTemplate, WebPart webPart, WebPartProperty webPartProperty, Transformation transformation)
        {
            TemplateId = pageTemplate.Id;
            TemplateCodeName = pageTemplate.CodeName;
            TemplateDisplayName = pageTemplate.DisplayName;
            WebPart = webPart.ControlId;
            Property = webPartProperty.Name;
            Transformation = transformation.FullName;
            TransformationType = transformation.TransformationType.ToString();
        }

        public static string UniqueOrderByKey(TransformationUsageResult usageResult)
        {
            return $"{usageResult.TemplateId}{usageResult.TemplateCodeName}{usageResult.WebPart}{usageResult.Property}{usageResult.Transformation}";
        }
    }
}