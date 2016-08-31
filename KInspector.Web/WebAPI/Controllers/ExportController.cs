using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

using Kentico.KInspector.Core;

using Kentico.KInspector.Modules.Export;

namespace Kentico.KInspector.Web
{
    public class ExportController : ApiController
	{
        #region Export

        // GET api/export/GetExportTypes
        [ActionName("GetExportTypes")]
        public HttpResponseMessage GetExportTypes()
        {
            var types = ExportModuleLoader.Modules.Select(module => module.ModuleMetaData.ModuleCodeName).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, types);
        }

        // GET api/export/GetModuleExport
        [ActionName("GetModuleExport")]
        public HttpResponseMessage GetModuleExport([FromUri]IEnumerable<string> moduleNames, [FromUri]InstanceConfig config, [FromUri]string exportType)
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

            var module = ExportModuleLoader.Modules.FirstOrDefault(m => m.ModuleMetaData.ModuleCodeName == exportType);
            if (module == null)
            {
                throw new ArgumentException(nameof(exportType));
            }

            try
            {
                using (var memoryStream = module.GetExportStream(moduleNames, instanceInfo) as MemoryStream)
                {
                    if (memoryStream == null)
                    {
                        throw new Exception("Empty export file");
                    }

                    // Send stream
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new ByteArrayContent(memoryStream.ToArray());
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue(module.ModuleMetaData.ModuleFileMimeType);
                    response.Content.Headers.ContentLength = memoryStream.Length;
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    response.Content.Headers.ContentDisposition.FileName =
                        $"KInspectorExport_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.{module.ModuleMetaData.ModuleFileExtension}";
                    response.Headers.ConnectionClose = true;

                    memoryStream.Flush();

                    return response;
                }

            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, $"Error in processing modules. Error message: {e.Message}");
            }
        }

        #endregion
    }
}