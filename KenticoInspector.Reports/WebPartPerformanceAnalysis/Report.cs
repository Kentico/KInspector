using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using System;
using System.Collections.Generic;

namespace KenticoInspector.Reports.WebPartPerformanceAnalysis
{
    class Report : IReport
    {
        public string Codename => nameof(WebPartPerformanceAnalysis);

        public IList<Version> CompatibleVersions => new List<Version>
        {
            new Version("10.0"),
            new Version("11.0")
        };

        public IList<Version> IncompatibleVersions => new List<Version>();

        public string LongDescription => @"<p>Displays list of web parts where 'columns' property is not specified.</p>
<p>Web parts without specified 'columns' property must load all field from the database.</p>
<p>By specifying this property, you can significantly lower the data transmission from database to the server and improve the load times.</p>
<p>For more information, <a href=""https://docs.kentico.com/k12sp/configuring-kentico/optimizing-performance-of-portal-engine-sites/loading-data-efficiently"">see documentation</a>.";

        public string Name => "Web Part Performance Analysis";

        public string ShortDescription => "Shows potential optimization opportunities.";

        public IList<string> Tags => new List<string> {
            ReportTags.PortalEngine,
            ReportTags.Performance,
            ReportTags.WebParts,
        };

        public ReportResults GetResults(Guid InstanceGuid)
        {
            throw new NotImplementedException();
        }
    }
}
