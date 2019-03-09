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
        [HttpGet]
        public ActionResult<IEnumerable<InstanceConfiguration>> Get()
        {
            var fsics = new FileSystemInstanceConfigurationService();
            return fsics.GetItems();
        }

        [HttpGet("{guid}")]
        public ActionResult<InstanceConfiguration> Get(Guid guid)
        {
            var fsics = new FileSystemInstanceConfigurationService();
            return fsics.GetItem(guid);
        }

        [HttpDelete("{guid}")]
        public void Delete(Guid guid)

        {
            var fsics = new FileSystemInstanceConfigurationService();
            fsics.Delete(guid);
        }

        [HttpPost]
        public Guid Post([FromBody] InstanceConfiguration instanceConfiguration)
        {
            var fsics = new FileSystemInstanceConfigurationService();
            return fsics.Upsert(instanceConfiguration);
        }
    }
}