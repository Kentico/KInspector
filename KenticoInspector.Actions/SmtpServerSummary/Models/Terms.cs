using KenticoInspector.Core.Models;

namespace KenticoInspector.Actions.SmtpServerSummary.Models
{
    public class Terms
    {
        public Term InvalidOptions { get; internal set; }

        public Term ServersFromSettingsTable { get; internal set; }

        public Term ServersFromSmtpTable { get; internal set; }

        public Term ListSummary { get; internal set; }

        public Term ServerDisabled { get; internal set; }

        public Term SiteSettingDisabled { get; internal set; }
    }
}
