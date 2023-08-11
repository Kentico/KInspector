namespace KenticoInspector.Actions.SmtpServerSummary.Models
{
    public class SmtpFromSmtpServers
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string User { get; set; }

        public string Authentication { get; set; }

        public string DeliveryMethod { get; set; }

        public bool SSL { get; set; }

        public bool Global { get; set; }

        public bool Enabled { get; set; }
    }
}
