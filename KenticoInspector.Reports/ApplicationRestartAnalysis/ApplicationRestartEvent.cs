using System;
using System.Collections.Generic;
using System.Text;

namespace KenticoInspector.Reports.ApplicationRestartAnalysis
{
    public class ApplicationRestartEvent
    {
        public string EventCode { get; set; }
        public DateTime EventTime { get; set; }
        public string EventMachineName { get; set; }
    }
}
