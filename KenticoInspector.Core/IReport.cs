using KenticoInspector.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KenticoInspector.Core
{
    public interface IReport
    {
        string Codename { get; }
        IList<Version> CompatibleVersions { get; }
        IList<Version> IncompatibleVersions { get; }
        string LongDescription { get; }
        string Name { get; }
        string ShortDescription { get; }
        IList<string> Tags { get; }
        ReportResults GetResults(Guid InstanceGuid);
    }
}
