using KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Data;

namespace KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Results
{
    public class TemplateUsageResult
    {
        public string Site { get; set; }

        public string NodeAliasPath { get; set; }

        public string Culture { get; set; }

        public string Page { get; set; }

        public int NodeId { get; set; }

        public int TemplateId { get; set; }

        public TemplateUsageResult(Page page)
        {
            Site = page.Site.Name;
            Page = page.Name;
            NodeId = page.NodeId;
            NodeAliasPath = page.AliasPath;
            Culture = page.Culture.Name;
            TemplateId = page.TemplateId;
        }

        public static string OrderByKey(TemplateUsageResult usageResult)
        {
            return $"{usageResult.Site}{usageResult.NodeAliasPath}";
        }
    }
}