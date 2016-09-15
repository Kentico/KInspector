using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Kentico.KInspector.Core;

using NPOI.XWPF.UserModel;

namespace Kentico.KInspector.Modules.Export.Modules
{
	/// <summary>
	/// Class for export into MS Word document using docx format.
	/// Will append results to a template, if it exists in:
	/// \Data\Templates\KInspectorReportTemplate.docx
	/// </summary>
	public class ExportDocx : IExportModule
	{
		/// <summary>
		/// Metadata of the module.
		/// </summary>
		public ExportModuleMetaData ModuleMetaData => new ExportModuleMetaData("Word", "ExportDocx", "docx", "application/docx");

		/// <summary>
		/// Returns stream result of the export process.
		/// </summary>
		/// <param name="moduleNames">Modules to export.</param>
		/// <param name="instanceInfo">Instance for which to execute modules.</param>
		/// <returns>Result stream</returns>
		public Stream GetExportStream(IEnumerable<string> moduleNames, IInstanceInfo instanceInfo)
		{
			// Create docx
			XWPFDocument document = null;
			try
			{
				// Open docx template
				document = new XWPFDocument(new FileInfo(@"Data\Templates\KInspectorReportTemplate.docx").OpenRead());
			}
			catch
			{
				// Create blank
				document = new XWPFDocument();
			}


			// Create sumary paragraph containing results of text modules, and sumary of all other modules.
			document.CreateParagraph("Result summary");
			XWPFTable resultSummary = document.CreateTable();
			resultSummary.GetRow(0).FillRow("Module", "Result", "Comment", "Description");

			// Run every module and write its result.
			foreach (string moduleName in moduleNames.Distinct())
			{
				var module = ModuleLoader.GetModule(moduleName);
				var result = module.GetResults(instanceInfo);
				var meta = module.GetModuleMetadata();

				switch (result.ResultType)
				{
					case ModuleResultsType.String:
						resultSummary.CreateRow().FillRow(moduleName, result.Result as string, result.ResultComment, meta.Comment);
						break;
					case ModuleResultsType.List:
						document.CreateParagraph(moduleName);
						document.CreateParagraph(result.ResultComment);
						document.CreateTable().FillTable(result.Result as IEnumerable<string>);
						resultSummary.CreateRow().FillRow(moduleName, "See details bellow", result.ResultComment, meta.Comment);
						break;
					case ModuleResultsType.Table:
						document.CreateParagraph(moduleName);
						document.CreateParagraph(result.ResultComment);
						document.CreateTable().FillRows(result.Result as DataTable);
						resultSummary.CreateRow().FillRow(moduleName, "See details bellow", result.ResultComment, meta.Comment);
						break;
					case ModuleResultsType.ListOfTables:
						document.CreateParagraph(moduleName);
						document.CreateParagraph(result.ResultComment);
						DataSet data = result.Result as DataSet;
						if (data == null)
						{
							resultSummary.CreateRow().FillRow(moduleName, "Internal error: Invalid DataSet", result.ResultComment, meta.Comment);
							break;
						}

						foreach (DataTable tab in data.Tables)
						{
							// Create header
							document.CreateParagraph(tab.TableName);

							// Write data
							document.CreateTable().FillRows(tab);
						}

						resultSummary.CreateRow().FillRow(moduleName, "See details bellow", result.ResultComment, meta.Comment);
						break;
					default:
						resultSummary.CreateRow().FillRow(moduleName, "Internal error: Unknown module", result.ResultComment, meta.Comment);
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
