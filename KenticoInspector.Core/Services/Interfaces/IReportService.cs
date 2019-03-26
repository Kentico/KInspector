﻿using KenticoInspector.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KenticoInspector.Core.Services.Interfaces
{
    public interface IReportService : IService
    {
        IReport GetReport(string codename);

        ReportResults GetReportResults(string reportCodename, Guid instanceGuid);

        IEnumerable<IReport> GetReports(ReportFilter reportFilter = null);
    }
}
