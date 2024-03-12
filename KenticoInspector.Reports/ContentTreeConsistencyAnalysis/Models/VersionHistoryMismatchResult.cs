using Newtonsoft.Json;

using System;
using System.Globalization;
using System.Text;

namespace KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models
{
    public class VersionHistoryMismatchResult
    {
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

        public VersionHistoryMismatchResult(int documentID, string fieldName, string fieldType, string versionHistoryXmlValue, object coupledDataColumnValue)
        {
            DocumentNodeId = documentID;
            FieldName = fieldName;
            ProcessItemValues(fieldType, versionHistoryXmlValue, coupledDataColumnValue);
        }

        private void ProcessItemValues(string fieldType, string versionHistoryXmlValue, object coupledDataColumnValue)
        {
            var hasAtLeastOneNullValue = versionHistoryXmlValue == null || coupledDataColumnValue == null;
            if (hasAtLeastOneNullValue)
            {
                DocumentValue = coupledDataColumnValue?.ToString();
                VersionHistoryValue = versionHistoryXmlValue;
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

            var formatting = new StringBuilder("0.");
            for (int i = 0; i < precision; i++)
            {
                formatting.Append('0');
            }

            VersionHistoryValue = versionHistoryValue.ToString(formatting.ToString());
        }

        private void ProcessDateTimeValues(string versionHistoryXmlValue, object coupledDataColumnValue)
        {
            DateTimeOffset versionHistoryValue;
            DateTime coupledDataDateTime = DateTime.Parse(coupledDataColumnValue.ToString());
            if (DateTimeOffset.TryParse(versionHistoryXmlValue, out versionHistoryValue) && coupledDataDateTime.Year != 1)
            {
                var assumedOffset = versionHistoryValue.Offset;
                var documentValueAdjusted = new DateTimeOffset(coupledDataDateTime, assumedOffset);
                DocumentValue = documentValueAdjusted.ToString();
                VersionHistoryValue = versionHistoryValue.ToString();
            }
            else
            {
                DocumentValue = versionHistoryXmlValue.ToString();
                VersionHistoryValue = versionHistoryXmlValue.ToString();
            }
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