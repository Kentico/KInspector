using KenticoInspector.Core.Models;
using KenticoInspector.Core.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace KenticoInspector.WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstancesController : ControllerBase
    {
        private readonly IInstanceRepository _instanceRepository;

        public InstancesController(IInstanceRepository instanceRepository)
        {
            _instanceRepository = instanceRepository;
        }

        [HttpDelete("{guid}")]
        public void Delete(Guid guid)
        {
            _instanceRepository.DeleteInstance(guid);
        }

        [HttpGet]
        public ActionResult<IEnumerable<Instance>> Get()
        {
            return _instanceRepository.GetInstances();
        }

        [HttpGet("{guid}")]
        public ActionResult<Instance> Get(Guid guid)
        {
            return _instanceRepository.GetInstance(guid);
        }

        [HttpPost]
        public Instance Post([FromBody] Instance instance)
        {
            return _instanceRepository.UpsertInstance(instance);
        }
    }
}