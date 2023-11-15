using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public Task<InstanceDetails> Details(Guid instanceGuid)
        {
            return Task.FromResult(_instanceService.GetInstanceDetails(instanceGuid));
        }

        [HttpDelete("{instanceGuid}")]
        public void Delete(Guid instanceGuid)
        {
            _instanceService.DeleteInstance(instanceGuid);
        }

        [HttpGet]
        public Task<List<Instance>> Get()
        {
            var instances = _instanceService.GetInstances();
            return Task.FromResult(instances.ToList());
        }

        [HttpGet("{instanceGuid}")]
        public Task<Instance> Get(Guid instanceGuid)
        {
            return Task.FromResult(_instanceService.GetInstance(instanceGuid));
        }

        [HttpPost]
        public Task<Instance> Post([FromBody] Instance instance)
        {
            return Task.FromResult(_instanceService.UpsertInstance(instance));
        }
    }
}