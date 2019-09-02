using System;
using System.Collections.Generic;

using KenticoInspector.Core.Models;

namespace KenticoInspector.Core.Modules
{
    public interface IReport : IModule
    {
        ReportResults GetResults();
    }
}