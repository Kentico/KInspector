using System.Xml;

namespace KInspector.Reports.ContentTreeConsistencyAnalysis.Models
{
    public class CmsClassItem
    {
        private string? _classIdColumn = null;
        private List<CmsClassField>? _classFields = null;
        
        public string? ClassDisplayName { get; set; }

        public XmlDocument? ClassFormDefinitionXml { get; set; }

        public int ClassID { get; set; }

        public string? ClassName { get; set; }

        public string? ClassTableName { get; set; }

        public List<CmsClassField>? ClassFields
        {
            get
            {
                if (_classFields is null && ClassFormDefinitionXml is not null)
                {
                    _classFields = GetFieldsFromXml();
                }

                return _classFields;
            }
        }

        public string? ClassIDColumn
        {
            get
            {
                if (_classIdColumn is null && ClassFormDefinitionXml is not null)
                {
                    _classIdColumn = ClassFields?.Where(x => x.IsIdColumn).Select(x => x.Column).FirstOrDefault();
                }

                return _classIdColumn;
            }
        }

        private List<CmsClassField> GetFieldsFromXml()
        {
            var fields = new List<CmsClassField>();
            var fieldsXml = ClassFormDefinitionXml?.SelectNodes("/form/field");
            if (fieldsXml is null)
            {
                return fields;
            }

            foreach (XmlNode field in fieldsXml)
            {
                var isIdColumnRaw = field.Attributes?["isPK"]?.Value;
                var isIdColumn = !string.IsNullOrWhiteSpace(isIdColumnRaw) ? bool.Parse(isIdColumnRaw) : false;

                fields.Add(new CmsClassField
                {
                    Caption = field.SelectSingleNode("properties/fieldcaption")?.InnerText,
                    Column = field.Attributes?["column"]?.Value,
                    ColumnType = field.Attributes?["columntype"]?.Value,
                    DefaultValue = field.SelectSingleNode("properties/defaultvalue")?.InnerText,
                    IsIdColumn = isIdColumn
                });
            }

            return fields;
        }
    }
}