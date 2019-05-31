﻿using System;
using System.Collections.Generic;
using System.Text;

namespace KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models
{
    public class CmsDocumentNode
    {
        public int DocumentID { get; set; }
        public string DocumentName { get; set; }
        public string DocumentNamePath { get; set; }
        public int DocumentNodeID { get; set; }
    }
}
