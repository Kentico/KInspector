using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

using Kentico.KInspector.Core;
using Kentico.KInspector.Modules;

namespace Kentico.KInspector.Web
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
		public HttpResponseMessage GetModulesMetadata([FromUri]InstanceConfig config, [FromUri] string category = null)
		{
			try
			{
				var instance = new InstanceInfo(config);
				var version = instance.Version;

				// Get all modules of given version
				var modules = ModuleLoader.Modules
					.Select(x => x.GetModuleMetadata())
					.Where(x => x.SupportedVersions.Contains(version));

				// Filter modules by category - return either specified category, or the rest
				if (string.IsNullOrEmpty(category))
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

				if (!modules.Any())
				{
					return Request.CreateResponse(HttpStatusCode.BadRequest,
						string.Format("There are no modules available for version {0}.", version));
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
		public HttpResponseMessage GetSetupModulesMetadata([FromUri]InstanceConfig config)
		{
			try
			{
				var instance = new InstanceInfo(config);
				var version = instance.Version;

				// Get all modules of given version which are in 'Setup' category
				var modules = ModuleLoader.Modules
					.Select(x => x.GetModuleMetadata())
					.Where(x => x.SupportedVersions.Contains(version))
					.Where(x => x.Category != null && x.Category.StartsWith("Setup", StringComparison.InvariantCultureIgnoreCase));

				if (!modules.Any())
				{
					return Request.CreateResponse(HttpStatusCode.BadRequest,
						string.Format("There are no modules available for version {0}.", version));
				}

				return Request.CreateResponse(HttpStatusCode.OK, modules);
			}
			catch (Exception e)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
			}
		}


		// GET api/modules/GetModuleResult
		[ActionName("GetModuleResult")]
		public HttpResponseMessage GetModuleResult(string moduleName, [FromUri]InstanceConfig config)
		{
			try
			{
				var instance = new InstanceInfo(config);
				var result = ModuleLoader.GetModule(moduleName).GetResults(instance);

				return Request.CreateResponse(HttpStatusCode.OK, result);
			}
			catch (Exception e)
			{
				return Request.CreateResponse(HttpStatusCode.InternalServerError,
					string.Format("Error in \"{0}\" module. Error message: {1}", moduleName, e.Message));
			}
		}


		// Get api/modules/GetKenticoVersion
		[ActionName("GetKenticoVersion")]
		public HttpResponseMessage GetKenticoVersion([FromUri]InstanceConfig config)
		{
			try
			{
				var instance = new InstanceInfo(config);
				var version = instance.Version;
				return Request.CreateResponse(HttpStatusCode.OK, version);
			}
			catch (Exception e)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
			}
		}

        // GET api/modules/GetModulesResults
        [ActionName("GetModulesResults")]
        public HttpResponseMessage GetModulesResults([FromUri]IEnumerable<string> moduleNames, [FromUri]InstanceConfig config, [FromUri]string exportType = "xlsx")
        {
            ExportHelper.ExportType type;
            if (!Enum.TryParse(exportType, true, out type))
            {
                // Unknown export type
                throw new ArgumentException(nameof(exportType));
            }

            var instanceInfo = new InstanceInfo(config);
            if (instanceInfo == null)
            {
                // TODO: Verify this
                throw new ArgumentException(nameof(config));
            }

            //try
            //{
                MemoryStream memoryStream = ExportHelper.GetExportStream(moduleNames, instanceInfo, type) as MemoryStream;
                if (memoryStream == null)
                {
                    throw new Exception("Empty export file");
                }

                // Send stream
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new ByteArrayContent(memoryStream.ToArray());
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(ExportHelper.GetMimeType(type));
                response.Content.Headers.ContentLength = memoryStream.Length;
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = $"KInspectorExport_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.{ExportHelper.GetExtension(type)}";
                response.Headers.ConnectionClose = true;

                memoryStream.Flush();

                return response;
            //}
            //catch (Exception e)
            //{
            //    return Request.CreateResponse(HttpStatusCode.InternalServerError, $"Error in processing modules. Error message: {e.Message}");
            //}
        }
	}
}