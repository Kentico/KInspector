namespace KenticoInspector.Actions.SiteStatusSummary.Models
{
    public class CmsSite
    {
        public int ID { get; internal set; }

        public string DisplayName { get; internal set; }

        public string Description { get; internal set; }

        public string AdministrationDomain { get; internal set; }

        public string PresentationUrl { get; internal set; }

        public bool Running { get; internal set; }
    }
}
