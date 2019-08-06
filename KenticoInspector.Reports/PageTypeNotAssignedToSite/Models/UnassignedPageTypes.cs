namespace KenticoInspector.Reports.PageTypeNotAssignedToSite.Models
{
    public class UnassignedPageTypes
    {
        public string ClassName { get; set; }

        public string SiteName { get; set; }

        public int NodeSiteID { get; set; }

        public int NodeClassID { get; set; }
    }
}