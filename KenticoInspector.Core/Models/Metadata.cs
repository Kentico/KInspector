namespace KenticoInspector.Core.Models
{
    public class Metadata<TLabels> where TLabels : new()
    {
        public Names Names { get; set; }

        public Descriptions Descriptions { get; set; }

        public TLabels Labels { get; set; }
    }

    public class Names
    {
        public string Short { get; set; }
    }

    public class Descriptions
    {
        public string Short { get; set; }

        public string Long { get; set; }
    }
}