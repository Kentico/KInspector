namespace KenticoInspector.Core.Models
{
    public class ReportMetadata<T> where T: new()
    {
        public ReportDetails Details { get; set; }

        public T Terms { get; set; }
    }
}