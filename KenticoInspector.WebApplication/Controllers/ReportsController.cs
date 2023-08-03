using System;
using System.Collections.Generic;

using KenticoInspector.Core;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace KenticoInspector.WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("{instanceGuid}")]
        public ActionResult<IEnumerable<IReport>> Get(Guid instanceGuid)
        {
            return Ok(_reportService.GetReports(instanceGuid));
        }

        [HttpGet("{codename}/results/{instanceGuid}")]
        public ActionResult<ReportResults> Get(string codename, Guid instanceGuid)
        {
            return _reportService.GetReportResults(codename, instanceGuid);
        }
    }
}