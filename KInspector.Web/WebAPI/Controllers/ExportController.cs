using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

using Kentico.KInspector.Core;

using Kentico.KInspector.Modules.Export;

namespace Kentico.KInspector.Web
{
	/// <summary>
	/// Controller handling export functionality.
	/// </summary>
	public class ExportController : ApiController
	{
		/// <summary>
		/// GET api/export/GetExportModulesMetadata
		/// </summary>
		[ActionName("GetExportModulesMetadata")]
		public HttpResponseMessage GetExportModulesMetadata()
		{
			try
			{
				// Get all modules of given version
				var modules = ExportModuleLoader.Modules.Select(m => m.ModuleMetaData).ToList();
				if (!modules.Any())
				{
					return Request.CreateResponse(HttpStatusCode.BadRequest, "There are no export modules available.");
				}

				return Request.CreateResponse(HttpStatusCode.OK, modules);
			}
			catch (Exception e)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
			}
		}

		/// <summary>
		/// GET api/export/GetModuleExport
		/// </summary>
		[ActionName("GetModuleExport")]
		public HttpResponseMessage GetModuleExport([FromUri]IEnumerable<string> moduleNames, [FromUri]InstanceConfig config, [FromUri]string exportModuleCodeName)
		{
			if (config == null)
			{
				throw new ArgumentNullException(nameof(config));
			}

			var instanceInfo = new InstanceInfo(config);
			if (instanceInfo == null)
			{
				throw new ArgumentException(nameof(config));
			}

			if (moduleNames == null)
			{
				throw new ArgumentNullException(nameof(moduleNames));
			}

			var module = ExportModuleLoader.Modules.FirstOrDefault(m => m.ModuleMetaData.ModuleCodeName == exportModuleCodeName);
			if (module == null)
			{
				throw new ArgumentException(nameof(exportModuleCodeName));
			}

			try
			{
				var stream = module.GetExportStream(moduleNames.Distinct(), instanceInfo);
				if (stream == null)
				{
					throw new Exception("Empty export file");
				}

				// Send stream
				var response = Request.CreateResponse(HttpStatusCode.OK);
				response.Content = new StreamContent(stream);
				response.Content.Headers.ContentType = new MediaTypeHeaderValue(module.ModuleMetaData.ModuleFileMimeType);
				response.Content.Headers.ContentLength = stream.Length;
				response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
				{
					FileName = $"KInspectorExport_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.{module.ModuleMetaData.ModuleFileExtension}"
				};
				response.Headers.ConnectionClose = true;

				return response;
			}
			catch (Exception e)
			{
				return Request.CreateResponse(HttpStatusCode.InternalServerError, $"Error in processing modules. Error message: {e.Message}");
			}
		}
	}
}