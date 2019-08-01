using KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Data;

namespace KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Results
{
    public class TemplateUsageResult
    {
        public string Site { get; }

        public string NodeAliasPath { get; }

        public string Culture { get; }

        public string Page { get; }

        public int NodeId { get; }

        public int TemplateId { get; }

        public TemplateUsageResult(Page page)
        {
            Site = page.Site.Name;
            Page = page.Name;
            NodeId = page.NodeId;
            NodeAliasPath = page.AliasPath;
            Culture = page.Culture.Name;
            TemplateId = page.TemplateId;
        }
    }
}