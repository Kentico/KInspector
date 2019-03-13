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
        private readonly IInstanceConfigurationService _instanceConfigurationService;
        private readonly IInstanceService _instanceService;

        public InstancesController(IInstanceConfigurationService instanceConfigurationService, IInstanceService instanceService)
        {
            _instanceConfigurationService = instanceConfigurationService;
            _instanceService = instanceService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<InstanceConfiguration>> Get()
        {
            return _instanceConfigurationService.GetItems();
        }

        [HttpGet("{guid}")]
        public ActionResult<InstanceConfiguration> Get(Guid guid)
        {
            var instanceConfiguration = _instanceConfigurationService.GetItem(guid);
            var instance = _instanceService.GetInstance(instanceConfiguration);
            return instanceConfiguration;
        }

        [HttpDelete("{guid}")]
        public void Delete(Guid guid)
        {
            _instanceConfigurationService.Delete(guid);
        }

        [HttpPost]
        public Guid Post([FromBody] InstanceConfiguration instanceConfiguration)
        {
            return _instanceConfigurationService.Upsert(instanceConfiguration);
        }
    }
}