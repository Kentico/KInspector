using KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Data;

namespace KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Results
{
    public class TransformationUsageResult
    {
        public int TemplateId { get; }

        public string TemplateCodeName { get; }

        public string TemplateDisplayName { get; }

        public string WebPart { get; }

        public string Property { get; }

        public string Transformation { get; }

        public string TransformationType { get; }

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
    }
}