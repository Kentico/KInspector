using System.ComponentModel;

namespace KInspector.Actions.StopRunningSites.Models
{
    public class Options
    {
        [DisplayName("Site ID")]
        [Description("The site ID to stop.")]
        public int? SiteId { get; set; }
    }
}
