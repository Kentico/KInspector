namespace KInspector.Actions.DisableStagingServers.Models
{
    public class StagingServer
    {
        public int ID { get; set; }

        public string? Name { get; set; }

        public string? Site { get; set; }

        public string? Url { get; set; }

        public bool Enabled { get; set; }
    }
}
