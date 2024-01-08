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
    public class InstancesController : ControllerBase
    {
        private readonly IInstanceService _instanceService;

        public InstancesController(IInstanceService instanceService)
        {
            _instanceService = instanceService;
        }

        [HttpGet("details/{instanceGuid}")]
        public ActionResult<InstanceDetails> Details(Guid instanceGuid)
        {
            return _instanceService.GetInstanceDetails(instanceGuid);
        }

        [HttpDelete("{instanceGuid}")]
        public void Delete(Guid instanceGuid)
        {
            _instanceService.DeleteInstance(instanceGuid);
        }

        [HttpGet]
        public ActionResult<IEnumerable<Instance>> Get()
        {
            var instances = _instanceService.GetInstances();
            return instances.ToList();
        }

        [HttpGet("{instanceGuid}")]
        public ActionResult<Instance> Get(Guid instanceGuid)
        {
            return _instanceService.GetInstance(instanceGuid);
        }

        [HttpPost]
        public Instance Post([FromBody] Instance instance)
        {
            return _instanceService.UpsertInstance(instance);
        }
    }
}