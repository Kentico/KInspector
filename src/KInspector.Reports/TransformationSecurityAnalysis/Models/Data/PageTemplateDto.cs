namespace KInspector.Reports.TransformationSecurityAnalysis.Models.Data
{
    /// <summary>
    /// The page template contains its web parts configuration in <see cref="PageTemplateWebParts"/>. This configuration contains references to transformation code names.
    /// </summary>
    public class PageTemplateDto
    {
        public int PageTemplateID { get; set; }

        public string? PageTemplateCodeName { get; set; }

        public string? PageTemplateDisplayName { get; set; }

        public string? PageTemplateWebParts { get; set; }
    }
}