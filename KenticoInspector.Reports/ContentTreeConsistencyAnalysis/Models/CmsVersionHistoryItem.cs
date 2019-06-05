using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models
{
    public class CmsVersionHistoryItem
    {
        public int DocumentID { get; set; }
        public XmlDocument NodeXml { get; set; }
        public int VersionClassID { get; set; }
        public DateTime WasPublishedFrom { get; set; }
    }
}
