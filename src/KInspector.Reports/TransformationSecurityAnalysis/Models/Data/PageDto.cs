namespace KInspector.Reports.TransformationSecurityAnalysis.Models.Data
{
    /// <summary>
    /// Represents a culture version of a node. References a <see cref="PageTemplateDto"/> in <see cref="DocumentPageTemplateID"/>.
    /// </summary>
    public class PageDto
    {
        public int NodeID { get; set; }

        public string? DocumentName { get; set; }

        public string? DocumentCulture { get; set; }

        public string? NodeAliasPath { get; set; }

        public int NodeSiteID { get; set; }

        public int DocumentPageTemplateID { get; set; }
    }
}