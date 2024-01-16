using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using KenticoInspector.Core.Models;
using KenticoInspector.Core.Modules;
using KenticoInspector.Core.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace KenticoInspector.WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IModuleService moduleService;

        public ReportsController(IModuleService moduleService)
        {
            this.moduleService = moduleService;
        }

        [HttpGet("{instanceGuid}")]
        public Task<IEnumerable<IReport>> Get(Guid instanceGuid)
        {
            return Task.FromResult(moduleService.GetReports(instanceGuid));
        }

        [HttpGet("{codename}/results/{instanceGuid}")]
        public Task<ReportResults> Get(string codename, Guid instanceGuid)
        {
            return Task.FromResult(moduleService.GetReportResults(codename, instanceGuid));
        }
    }
}