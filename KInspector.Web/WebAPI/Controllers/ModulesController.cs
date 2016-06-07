using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Kentico.KInspector.Core;
using Kentico.KInspector.Extensions;

using Novacode;

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
        public HttpResponseMessage GetModulesResults([FromUri]IEnumerable<string> moduleNames, [FromUri]InstanceConfig config)
        {
            try
            {
                var instance = new InstanceInfo(config);

                // Create DocX
                DocX doc = DocX.Load(@"Templates\KInspectorReportTemplate.docx");
                if (doc == null)
                {
                    throw new Exception("Could open find export template \"Templates\\KInspectorReportTemplate.docx\"");
                }

                // Process "macros"
                Dictionary<string, string> macros = new Dictionary<string, string>
                {
                    {"SiteName", Convert.ToString(instance.Uri)},
                    {"SiteVersion", Convert.ToString(instance.Version)},
                    {"SiteDirectory", Convert.ToString(instance.Directory)}
                };

                foreach (var macro in macros)
                {
                    doc.ReplaceText($"{{% {macro.Key} %}}", macro.Value);
                }

                foreach (string moduleName in moduleNames)
                {
                    var result = ModuleLoader.GetModule(moduleName).GetResults(instance);
                    switch (result.ResultType)
                    {
                        case ModuleResultsType.String:
                            doc.InsertParagraph($"{moduleName}: {result.Result} ({result.ResultComment})");
                            break;
                        case ModuleResultsType.List:
                            doc.InsertParagraphs($"{moduleName}: ({result.ResultComment})");
                            List<string> resultList = result.Result as List<string>;
                            if (resultList != null)
                            {
                                var tbl = doc.InsertTable(resultList.Count, 1);
                                for (int row = 0; row < tbl.RowCount; row++)
                                {
                                    tbl.Rows[row].Cells[0].Paragraphs[0].InsertText(resultList[row]);
                                }
                            }
                            break;
                        case ModuleResultsType.Table:
                            doc.InsertParagraphs($"{moduleName}: ({result.ResultComment})");
                            DataTable resultTable = result.Result as DataTable;
                            doc.InsertDataTable(resultTable);
                            break;
                        case ModuleResultsType.ListOfTables:
                            doc.InsertParagraphs($"{moduleName}: ({result.ResultComment})");
                            DataSet data = result.Result as DataSet;
                            foreach (DataTable tab in data.Tables)
                            {
                                doc.InsertDataTable(tab);
                            }
                            break;
                        default:
                            continue;
                    }
                }

                // Save as stream
                MemoryStream memoryStream = new MemoryStream();
                doc.SaveAs(memoryStream);

                // Send stream
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new ByteArrayContent(memoryStream.ToArray());
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/docx");
                response.Content.Headers.ContentLength = memoryStream.Length;
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = $"KInspectorExport{DateTime.Now.ToFileTimeUtc()}.docx";
                response.Headers.ConnectionClose = true;

                memoryStream.Flush();

                return response;
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, $"Error in processing modules. Error message: {e.Message}");
            }
        }
	}
}