namespace KenticoInspector.Actions.SiteStatusSummary.Models
{
    public class CmsSite
    {
        public int ID { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public string AdministrationDomain { get; set; }

        public string PresentationUrl { get; set; }

        public bool Running { get; set; }
    }
}
