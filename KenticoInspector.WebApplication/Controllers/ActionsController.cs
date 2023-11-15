using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using KenticoInspector.Core.Models;
using KenticoInspector.Core.Modules;
using KenticoInspector.Core.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace KenticoInspector.WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActionsController : ControllerBase
    {
        private readonly IModuleService moduleService;

        public ActionsController(IModuleService moduleService)
        {
            this.moduleService = moduleService;
        }

        [HttpGet("{instanceGuid}")]
        public Task<IEnumerable<IAction>> Get(Guid instanceGuid)
        {
            return Task.FromResult(moduleService.GetActions(instanceGuid));
        }

        [HttpPost("{codename}/execute/{instanceGuid}")]
        public async Task<ActionResults> Excecute(string codename, Guid instanceGuid)
        {
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var optionsJson = await reader.ReadToEndAsync();
                return moduleService.ExecuteAction(codename, instanceGuid, optionsJson);
            }
        }
    }
}