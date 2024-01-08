using System;
using System.Xml;

namespace KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models
{
    public class CmsVersionHistoryItem
    {
        private int _coupledDataId = -1;

        public int VersionHistoryID { get; set; }

        public int DocumentID { get; set; }

        public XmlDocument NodeXml { get; set; }

        public int VersionClassID { get; set; }

        public DateTime WasPublishedFrom { get; set; }

        public int CoupledDataID
        {
            get
            {
                if (_coupledDataId == -1 && NodeXml != null)
                {
                    _coupledDataId = GetCoupledDataId();
                }

                return _coupledDataId;
            }
        }

        private int GetCoupledDataId()
        {
            var foreignKeyRaw = NodeXml.SelectSingleNode("//DocumentForeignKeyValue")?.InnerText;
            int foreignKey;

            return int.TryParse(foreignKeyRaw, out foreignKey) ? foreignKey : -1;
        }
    }
}