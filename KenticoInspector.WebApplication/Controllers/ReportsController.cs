﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KenticoInspector.Core;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using Microsoft.AspNetCore.Http;
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
            _reportService= reportService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<IReport>> Get()
        {
            return _reportService.GetReports().ToList();
        }

        // GET api/reports/5
        [HttpGet("{codename}")]
        public ActionResult<IReport> Get(string codename)
        {
            return Ok(_reportService.GetReport(codename));
        }

        // POST api/values
        [HttpGet("{codename}/results/{instanceGuid}")]
        public ActionResult<ReportResults> Get(string codename, Guid instanceGuid)
        {
            return _reportService.GetReportResults(codename, instanceGuid);
        }
    }
}