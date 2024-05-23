﻿namespace KInspector.Reports.PageTypeFieldAnalysis.Models
{
    public class CmsPageTypeField
    {
        public string? PageTypeCodeName { get; set; }

        public string? FieldName { get; set; }

        public string? FieldDataType { get; set; }

        public override bool Equals(object? obj)
        {
            var comparingField = obj as CmsPageTypeField;
            if (comparingField is null)
            {
                return base.Equals(obj);
            }

            var fieldsAreEqual = comparingField.FieldName == FieldName && comparingField.FieldDataType == FieldDataType;

            return fieldsAreEqual;
        }

        public override int GetHashCode()
        {
            int hCode = (FieldName + FieldDataType).GetHashCode();

            return hCode;
        }
    }
}