namespace KInspector.Core.Models
{
    /// <summary>
    /// Represents a Kentico website from the CMS_Site table.
    /// </summary>
    public class Site
    {
        /// <summary>
        /// The site administration domain name.
        /// </summary>
        public string? DomainName { get; set; }

        /// <summary>
        /// The site GUID.
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        /// The site ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The site name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The site presentation URL.
        /// </summary>
        public string? PresentationUrl { get; set; }

        /// <summary>
        /// The site status.
        /// </summary>
        public string? Status { get; set; }
    }
}