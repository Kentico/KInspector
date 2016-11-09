using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Kentico.KInspector.Core;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Kentico.KInspector.Modules.Export.Modules
{
	/// <summary>
	/// Class for export into MS Excel document using xlsx format.
	/// </summary>
	public class ExportXlsx : IExportModule
	{
		/// <summary>
		/// Metadata of the module.
		/// </summary>
		public ExportModuleMetaData ModuleMetaData => new ExportModuleMetaData("Excel", "ExportXlsx", "xlsx", "application/xlsx");

		/// <summary>
		/// Returns stream result of the export process.
		/// </summary>
		/// <param name="moduleNames">Modules to export.</param>
		/// <param name="instanceInfo">Instance for which to execute modules.</param>
		/// <returns>Result stream</returns>
		public Stream GetExportStream(IEnumerable<string> moduleNames, IInstanceInfo instanceInfo)
		{
			// Create xlsx
			IWorkbook document = new XSSFWorkbook();

			// Create sheet to store results of text modules, and sumary of all other modules.
			ISheet resultSummary = document.CreateSheet("Result summary");
			resultSummary.CreateRow("Module", "Result", "Comment", "Description");

			// Run every module and write its result.
			foreach (string moduleName in moduleNames.Distinct())
			{
				var module = ModuleLoader.GetModule(moduleName);
				var result = module.GetResults(instanceInfo);
				var meta = module.GetModuleMetadata();

				switch (result.ResultType)
				{
					case ModuleResultsType.String:
						resultSummary.CreateRow(moduleName, result.Result as string, result.ResultComment, meta.Comment);
						break;
					case ModuleResultsType.List:
						document.CreateSheet(moduleName).CreateRows(result.Result as IEnumerable<string>);
						resultSummary.CreateRow(moduleName, "See details in tab", result.ResultComment, meta.Comment);
						break;
					case ModuleResultsType.Table:
						document.CreateSheet(moduleName).CreateRows(result.Result as DataTable);
						resultSummary.CreateRow(moduleName, "See details in tab", result.ResultComment, meta.Comment);
						break;
					case ModuleResultsType.ListOfTables:
						DataSet data = result.Result as DataSet;
						if (data == null)
						{
							resultSummary.CreateRow(moduleName, "Internal error: Invalid DataSet", result.ResultComment, meta.Comment);
							break;
						}

						ISheet currentSheet = document.CreateSheet(moduleName);
						foreach (DataTable tab in data.Tables)
						{
							// Create header
							currentSheet.CreateRow(tab.TableName);

							// Write data
							currentSheet.CreateRows(tab);

							// Create divider
							currentSheet.CreateRow();
						}

						resultSummary.CreateRow(moduleName, "See details in tab", result.ResultComment, meta.Comment);
						break;
					default:
						resultSummary.CreateRow(moduleName, "Internal error: Unknown module", result.ResultComment, meta.Comment);
						continue;
				}
			}

			// XWPFDocument.Write closes the stream. NpoiMemoryStream is used to prevent it.
			NpoiMemoryStream stream = new NpoiMemoryStream(false);
			document.Write(stream);
			stream.Seek(0, SeekOrigin.Begin);
			stream.AllowClose = true;

			return stream;
		}
	}
}
