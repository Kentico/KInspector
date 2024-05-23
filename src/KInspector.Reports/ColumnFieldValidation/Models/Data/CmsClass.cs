using System.Xml.Linq;

namespace KInspector.Reports.ColumnFieldValidation.Models.Data
{
    public class CmsClass
    {
        public int ClassID { get; set; }

        public string? ClassName { get; set; }

        public string? ClassDisplayName { get; set; }

        public string? ClassTableName { get; set; }

        public XDocument? ClassXmlSchema { get; set; }
    }
}