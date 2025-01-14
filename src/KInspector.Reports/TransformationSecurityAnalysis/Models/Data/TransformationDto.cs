namespace KInspector.Reports.TransformationSecurityAnalysis.Models.Data
{
    /// <summary>
    /// Transformation of type <see cref="Type"/>.
    /// </summary>
    public class TransformationDto
    {
        public string? TransformationName { get; set; }

        public string? TransformationCode { get; set; }

        public TransformationType Type { get; set; }

        public string? ClassName { get; set; }
    }
}