using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace KenticoInspector.WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstancesController : ControllerBase
    {
        IInstanceConfigurationService instanceConfigurationService;

        public InstancesController(IInstanceConfigurationService instanceConfigurationService)
        {
            this.instanceConfigurationService = instanceConfigurationService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<InstanceConfiguration>> Get()
        {
            return instanceConfigurationService.GetItems();
        }

        [HttpGet("{guid}")]
        public ActionResult<InstanceConfiguration> Get(Guid guid)
        {
            return instanceConfigurationService.GetItem(guid);
        }

        [HttpDelete("{guid}")]
        public void Delete(Guid guid)
        {
            instanceConfigurationService.Delete(guid);
        }

        [HttpPost]
        public Guid Post([FromBody] InstanceConfiguration instanceConfiguration)
        {
            return instanceConfigurationService.Upsert(instanceConfiguration);
        }
    }
}