using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

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

        [HttpGet("details/{guid}")]
        public ActionResult<InstanceDetails> Details(Guid guid)
        {
            return _instanceService.GetInstanceDetails(guid);
        }

        [HttpDelete("{guid}")]
        public void Delete(Guid guid)
        {
            _instanceService.DeleteInstance(guid);
        }

        [HttpGet]
        public ActionResult<IEnumerable<Instance>> Get()
        {
            var instances = _instanceService.GetInstances();
            return instances.ToList();
        }

        [HttpGet("{guid}")]
        public ActionResult<Instance> Get(Guid guid)
        {
            return _instanceService.GetInstance(guid);
        }

        [HttpPost]
        public Instance Post([FromBody] Instance instance)
        {
            return _instanceService.UpsertInstance(instance);
        }
    }
}