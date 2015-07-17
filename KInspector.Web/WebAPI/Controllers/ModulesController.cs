using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using KInspector.Core;

namespace KInspector.Web.WebAPI.Controllers
{
    public class ModulesController : ApiController
    {
        /// <summary>
        /// Categories, which have their own page in the UI. The rest
        /// is displayed together on the 'Analysis' page.
        /// </summary>
        public static string[] SeparateCategories = { "Security", "Setup" };


        // GET api/modules/GetModulesMetadata
        [ActionName("GetModulesMetadata")]
        public HttpResponseMessage GetModulesMetadata([FromUri]KenticoInstanceConfig config, [FromUri] string category = null)
        {
            try
            {
                DatabaseService dbService = new DatabaseService(config.Server, config.Database, config.User, config.Password);
                var kenticoVersion = GetKenticoVersion(dbService);

                // Get all modules of given version
                var modules = ModuleLoader.Modules
                    .Select(x => x.GetModuleMetadata())
                    .Where(x => x.SupportedVersions.Contains(kenticoVersion));

                // Filter modules by category - return either specified category, or the rest
                if (String.IsNullOrEmpty(category))
                {
                    foreach (var separateCategory in SeparateCategories)
                    {
                        modules = modules.Where(x => x.Category == null || !x.Category.StartsWith(separateCategory, StringComparison.InvariantCultureIgnoreCase));
                    }
                }
                else
                {
                    modules = modules.Where(x => x.Category != null && x.Category.StartsWith(category, StringComparison.InvariantCultureIgnoreCase));
                }

                return Request.CreateResponse(HttpStatusCode.OK, modules);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }


        // GET api/modules/GetSetupModulesMetadata
        [ActionName("GetSetupModulesMetadata")]
        public HttpResponseMessage GetSetupModulesMetadata([FromUri]KenticoInstanceConfig config)
        {
            try
            {
                DatabaseService dbService = new DatabaseService(config.Server, config.Database, config.User, config.Password);
                var kenticoVersion = GetKenticoVersion(dbService);

                // Get all modules of given version which are in 'Setup' category
                var modules = ModuleLoader.Modules
                    .Select(x => x.GetModuleMetadata())
                    .Where(x => x.SupportedVersions.Contains(kenticoVersion))
                    .Where(x => x.Category != null && x.Category.StartsWith("Setup", StringComparison.InvariantCultureIgnoreCase));
                return Request.CreateResponse(HttpStatusCode.OK, modules);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }


        // GET api/modules/GetModuleResult
        [ActionName("GetModuleResult")]
        public HttpResponseMessage GetModuleResult(string moduleName, [FromUri]KenticoInstanceConfig config)
        {
            try
            {
                DatabaseService dbService = new DatabaseService(config.Server, config.Database, config.User, config.Password);
                var version = GetKenticoVersion(dbService);

                InstanceInfo instanceInfo = new InstanceInfo(version, new Uri(config.Url), new DirectoryInfo(config.Path));
                var result = ModuleLoader.GetModule(moduleName).GetResults(instanceInfo, dbService);

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, 
                    String.Format("Error in \"{0}\" module. Error message: {1}", moduleName, e.Message));
            }
        }


        // Get api/modules/GetKenticoVersion
        [ActionName("GetKenticoVersion")]
        public HttpResponseMessage GetKenticoVersion([FromUri]KenticoInstanceConfig config)
        {
            try
            {
                DatabaseService dbService = new DatabaseService(config.Server, config.Database, config.User, config.Password);
                var version = GetKenticoVersion(dbService);
                return Request.CreateResponse(HttpStatusCode.OK, version);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }


        /// <summary>
        /// Gets the version of Kentico.
        /// </summary>
        private static Version GetKenticoVersion(DatabaseService dbService)
        {
            string version = dbService.ExecuteAndGetScalar<string>("SELECT KeyValue FROM CMS_SettingsKey WHERE KeyName = 'CMSDBVersion'");
            return new Version(version);
        }
    }
}