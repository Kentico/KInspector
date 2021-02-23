using System;
using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace KenticoInspector.WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VersionsController : ControllerBase
    {
        private readonly IVersionService _versionService;

        public VersionsController(IVersionService versionService)
        {
            _versionService = versionService;
        }

        [HttpGet("GetCoreVersion")]
        public ActionResult<string> GetCoreVersion()
        {
            return _versionService.GetCoreProductVersion();
        }
    }
}