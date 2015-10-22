namespace Kentico.KInspector.Core
{
    /// <summary>
    /// Kentico instance configuration model gathering data from setup page.
    /// </summary>
    public class InstanceConfig
    {
        public string Url { get; set; }
        public string Path { get; set; }
        public string Database { get; set; }
        public string Server { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public bool IntegratedSecurity { get; set; }
    }
}
