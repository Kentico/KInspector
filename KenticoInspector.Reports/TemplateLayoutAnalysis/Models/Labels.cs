using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.TemplateLayoutAnalysis.Models
{
    public class Labels
    {
        public Label IdenticalPageLayouts { get; set; }

        public Label CountIdenticalPageLayoutFound { get; set; }

        public Label NoIdenticalPageLayoutsFound { get; set; }
    }
}