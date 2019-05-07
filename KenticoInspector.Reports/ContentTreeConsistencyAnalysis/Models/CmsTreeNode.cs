using System;
using System.Collections.Generic;
using System.Text;

namespace KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models
{
    public class CmsTreeNode
    {
        public string NodeAliasPath { get; set; }
        public int NodeID { get; set; }
        public int? NodeParentID { get; set; }
        public int NodeSiteID { get; set; }
        public int NodeLevel { get; set; }
    }
}
