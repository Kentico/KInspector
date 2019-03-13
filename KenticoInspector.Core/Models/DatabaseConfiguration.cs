namespace KenticoInspector.Core.Models
{
    public class DatabaseConfiguration
    {
        public string ServerName { get; set; }

        public string DatabaseName { get; set; }

        public bool IntegratedSecurity { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        public int? CommandTimeout { get; set; }
    }
}