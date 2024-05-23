using KInspector.Core.Models;

namespace KInspector.Reports.TemplateLayoutAnalysis.Models
{
    public class Terms
    {
        public Term? CountIdenticalPageLayoutFound { get; set; }

        public Term? IdenticalPageLayouts { get; set; }

        public Term? NoIdenticalPageLayoutsFound { get; set; }
    }
}