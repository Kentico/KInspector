using System.ComponentModel;

namespace KInspector.Actions.DisableStagingServers.Models
{
    public class Options
    {
        [DisplayName("Server ID")]
        [Description("The content staging server ID to disable.")]
        public int? ServerId { get; set; }
    }
}
