using Autofac;
using KenticoInspector.Core;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Repositories.Interfaces;
using KenticoInspector.Reports;
using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Infrastructure.Repositories
{
    class ReportRepository : IReportRepository
    {
        public IReport GetReport(string codename)
        {
            var allReports = LoadReports();
            return allReports.FirstOrDefault(x => x.Codename.ToLower() == codename.ToLower());
        }

        public IList<IReport> GetReports(ReportFilter filterSettings = null)
        {
            return LoadReports();
        }

        private IList<IReport> LoadReports() {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule(new CoreModule());
            containerBuilder.RegisterModule(new InfrastructureModule());
            containerBuilder.RegisterModule(new ReportsModule());
            var container = containerBuilder.Build();
            return container.Resolve<IList<IReport>>();
        }
    }
}
