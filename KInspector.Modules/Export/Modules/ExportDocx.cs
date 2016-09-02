using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kentico.KInspector.Core;

using NPOI.XWPF.UserModel;

namespace Kentico.KInspector.Modules.Export.Modules
{
    public class ExportDocx : IExportModule
    {
        public ExportModuleMetaData ModuleMetaData => new ExportModuleMetaData("Word", "ExportDocx", "docx", "application/docx");

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

            document.CreateParagraph("Result summary");
            XWPFTable resultSummary = document.CreateTable();
            resultSummary.GetRow(0).FillRow("Module", "Result", "Comment");

            foreach (string moduleName in moduleNames)
            {
                var result = ModuleLoader.GetModule(moduleName).GetResults(instanceInfo);
                switch (result.ResultType)
                {
                    case ModuleResultsType.String:
                        resultSummary.CreateRow().FillRow(moduleName, result.Result as string, result.ResultComment);
                        break;
                    case ModuleResultsType.List:
                        document.CreateParagraph(moduleName);
                        document.CreateParagraph(result.ResultComment);
                        document.CreateTable().FillTable(result.Result as IEnumerable<string>);
                        resultSummary.CreateRow().FillRow(moduleName, "See details bellow", result.ResultComment);
                        break;
                    case ModuleResultsType.Table:
                        document.CreateParagraph(moduleName);
                        document.CreateParagraph(result.ResultComment);
                        document.CreateTable().FillRows(result.Result as DataTable);
                        resultSummary.CreateRow().FillRow(moduleName, "See details bellow", result.ResultComment);
                        break;
                    case ModuleResultsType.ListOfTables:
                        document.CreateParagraph(moduleName);
                        document.CreateParagraph(result.ResultComment);
                        DataSet data = result.Result as DataSet;
                        if (data == null)
                        {
                            resultSummary.CreateRow().FillRow(moduleName, "Internal error: Invalid DataSet", result.ResultComment);
                            break;
                        }

                        foreach (DataTable tab in data.Tables)
                        {
                            document.CreateTable().FillRows(tab);
                        }

                        resultSummary.CreateRow().FillRow(moduleName, "See details bellow", result.ResultComment);
                        break;
                    default:
                        resultSummary.CreateRow().FillRow(moduleName, "Internal error: Unknown module", result.ResultComment);
                        continue;
                }
            }

            // XWPFDocument.Write closes the stream. NpoiMemoryStream is used to prevent it.
            NpoiMemoryStream stream = new NpoiMemoryStream(false);
            document.Write(stream);
            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            stream.AllowClose = true;
            
            return stream;
        }

    }
}
