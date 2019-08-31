using System;
using System.Collections.Generic;

using KenticoInspector.Core.Models;
using KenticoInspector.Core.Modules;
using KenticoInspector.Core.Services.Interfaces;

namespace KenticoInspector.Core
{
    public abstract class AbstractReport<T> : AbstractModule<T>, IReport where T : new()
    {
        public AbstractReport(IModuleMetadataService moduleMetadataService) : base(moduleMetadataService)
        {
        }

        public abstract ReportResults GetResults();
    }
}