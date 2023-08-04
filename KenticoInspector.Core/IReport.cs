using System;
using System.Collections.Generic;

using KenticoInspector.Core.Models;

namespace KenticoInspector.Core
{
    public interface IReport
    {
        string Codename { get; }

        IList<Version> CompatibleVersions { get; }

        IList<Version> IncompatibleVersions { get; }

        bool ModifiesData { get; }

        IList<string> Tags { get; }

        ReportResults GetResults();
    }
}