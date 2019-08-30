namespace KenticoInspector.Reports.PageTypeFieldAnalysis.Models
{
    public class CmsPageTypeField
    {
        public string PageTypeCodeName { get; set; }

        public string FieldName { get; set; }

        public string FieldDataType { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is CmsPageTypeField)
            {
                CmsPageTypeField comparingField = (CmsPageTypeField)obj;

                var fieldsAreEqual = comparingField.FieldName == FieldName && comparingField.FieldDataType == FieldDataType;

                return fieldsAreEqual;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            int hCode = (FieldName + FieldDataType).GetHashCode();

            return hCode;
        }
    }
}