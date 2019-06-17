using Newtonsoft.Json;
using System;
using System.Globalization;

namespace KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models
{
    public class VersionHistoryMismatchResult
    {
        public VersionHistoryMismatchResult(int documentID, string fieldName, string fieldType, string versionHistoryXmlValue, object coupledDataColumnValue)
        {
            DocumentNodeId = documentID;
            FieldName = fieldName;
            ProcessItemValues(fieldType, versionHistoryXmlValue, coupledDataColumnValue);
        }

        public int DocumentNodeId { get; set; }
        public string FieldName { get; set; }
        public string DocumentValue { get; set; }
        public string VersionHistoryValue { get; set; }

        [JsonIgnore]
        public bool FieldValuesMatch
        {
            get
            {
                return DocumentValue == VersionHistoryValue;
            }
        }

        private void ProcessItemValues(string fieldType, string versionHistoryXmlValue, object coupledDataColumnValue)
        {
            var hasAtLeastOneNullValue = versionHistoryXmlValue == null || coupledDataColumnValue == null;
            if (hasAtLeastOneNullValue)
            {
                DocumentValue = coupledDataColumnValue?.ToString();
                VersionHistoryValue = versionHistoryXmlValue;

                var areBothValuesNull = versionHistoryXmlValue == null && coupledDataColumnValue == null;
            }
            else
            {
                switch (fieldType)
                {
                    case FieldTypes.Boolean:
                        ProcessBoolValues(versionHistoryXmlValue, coupledDataColumnValue);
                        break;
                    case FieldTypes.DateTime:
                        ProcessDateTimeValues(versionHistoryXmlValue, coupledDataColumnValue);
                        break;
                    case FieldTypes.Decimal:
                        ProcessDecimalValues(versionHistoryXmlValue, coupledDataColumnValue);
                        break;
                    default:
                        DocumentValue = coupledDataColumnValue.ToString();
                        VersionHistoryValue = versionHistoryXmlValue;
                        break;
                }
            }
        }

        private void ProcessDecimalValues(string versionHistoryXmlValue, object coupledDataColumnValue)
        {
            var documentValue = (decimal)coupledDataColumnValue;
            DocumentValue = documentValue.ToString();

            var decimalSeparator = NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;
            var position = DocumentValue.IndexOf(decimalSeparator);
            var precision = (position == -1) ? 0 : DocumentValue.Length - position - 1;

            var versionHistoryValue = decimal.Parse(versionHistoryXmlValue);

            var formatting = "0.";
            for (int i = 0; i < precision; i++)
            {
                formatting += "0";
            }

            VersionHistoryValue = versionHistoryValue.ToString(formatting);
        }

        private void ProcessDateTimeValues(string versionHistoryXmlValue, object coupledDataColumnValue)
        {
            var versionHistoryValue = DateTimeOffset.Parse(versionHistoryXmlValue);
            var assumedOffset = versionHistoryValue.Offset;
            var documentValueAdjusted = new DateTimeOffset((DateTime)coupledDataColumnValue, assumedOffset);
            DocumentValue = documentValueAdjusted.ToString();
            VersionHistoryValue = versionHistoryValue.ToString();
        }

        private void ProcessBoolValues(string versionHistoryXmlValue, object coupledDataColumnValue)
        {
            var documentValue = (bool)coupledDataColumnValue;
            var versionHistoryValue = bool.Parse(versionHistoryXmlValue);
            DocumentValue = documentValue.ToString();
            VersionHistoryValue = versionHistoryValue.ToString();
        }
    }
}
