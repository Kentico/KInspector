using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KenticoInspector.Reports.ContentTreeConsistencyAnalysis
{
    class Report : IReport
    {
        public string Codename => "Content-Tree-Consistency-Analysis";

        public IList<Version> CompatibleVersions => new List<Version> {
            new Version("10.0"),
            new Version("11.0"),
            new Version("12.0")
        };

        public IList<Version> IncompatibleVersions => new List<Version>();

        public string LongDescription => @"
        <p>Checks that CMS_Tree and CMS_Document tables are without any consistency issues.</p>
        ";

        public string Name => "Content Tree Consistency Analysis";

        public string ShortDescription => "Performs consistency analysis for content items in the content tree";

        public IList<string> Tags => new List<string>()
        {
            ReportTags.Health,
            ReportTags.Consistency
        };

        public ReportResults GetResults(Guid InstanceGuid)
        {
            throw new NotImplementedException();
        }
    }
}
