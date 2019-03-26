using System;
using System.Collections.Generic;
using System.Text;

namespace KenticoInspector.Core.Models
{
    public class ReportFilter
    {
        public IList<string> Tags { get; set; }
        public bool ShowUntested { get; set; }
        public bool ShowIncompatible { get; set; }
    }
}
