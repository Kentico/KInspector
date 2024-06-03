using System.ComponentModel;

namespace KInspector.Actions.DisableSmtpServers.Models
{
    public class Options
    {
        [DisplayName("Site ID")]
        [Description("The site ID used to disable SMTP servers in the Settings application.")]
        public int? SiteId { get; set; }

        [DisplayName("Server ID")]
        [Description("The server ID to disable from the SMTP Servers application.")]
        public int? ServerId { get; set; }
    }
}
