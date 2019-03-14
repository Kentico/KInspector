namespace KenticoInspector.Core.Models
{
    public class DatabaseSettings
    {
        public string Database { get; set; }
        public bool IntegratedSecurity { get; set; }
        public string Password { get; set; }
        public string Server { get; set; }
        public string User { get; set; }
    }
}