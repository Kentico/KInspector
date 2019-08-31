using System;
using System.Collections.Generic;

using KenticoInspector.Core.Models;
using KenticoInspector.Core.Modules;

namespace KenticoInspector.Core.Services.Interfaces
{
    public interface IModuleService : IService
    {
        IReport GetReport(string codename);

        ReportResults GetReportResults(string reportCodename, Guid instanceGuid);

        IEnumerable<IReport> GetReports(Guid instanceGuid);

        IEnumerable<IAction> GetActions(Guid instanceGuid);

        IAction GetAction(string codename);
        ActionResults ExecuteAction<T>(string actionCodename, Guid instanceGuid, T options) where T : new();
    }
}