using System;
using System.Collections.Generic;
using System.Text;

namespace KenticoInspector.Reports.DatabaseTableSizeAnalysis
{
    public class DatabaseTableSizeAnalysis
    {
        public string TableName { get; set; }
        public int Rows { get; set; }
        public int SizeInMB { get; set; }
        public int BytesPerRow { get; set; }

    }
}
