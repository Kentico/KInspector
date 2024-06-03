using System.ComponentModel;

namespace KInspector.Actions.DisableWebFarmServers.Models
{
    public class Options
    {
        [DisplayName("Server ID")]
        [Description("The web farm server ID to disable.")]
        public int? ServerId { get; set; }
    }
}
