using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kentico.KInspector.Core;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Kentico.KInspector.Modules.Export.Modules
{
    public class ExportXlsx : IExportModule
    {
        public ExportModuleMetaData ModuleMetaData => new ExportModuleMetaData("Xlsx", "ExportXlsx", "xlsx", "application/xlsx");

        public Stream GetExportStream(IEnumerable<string> moduleNames, IInstanceInfo instanceInfo)
        {
            // Create xlsx
            IWorkbook document = new XSSFWorkbook();

            // Create sheet to store results of text modules
            ISheet resultSummary = document.CreateSheet("Result summary");
            resultSummary.CreateRow("Module", "Result", "Comment");

            foreach (string moduleName in moduleNames)
            {
                var result = ModuleLoader.GetModule(moduleName).GetResults(instanceInfo);

                switch (result.ResultType)
                {
                    case ModuleResultsType.String:
                        resultSummary.CreateRow(moduleName, result.Result as string, result.ResultComment);
                        break;
                    case ModuleResultsType.List:
                        document.CreateSheet(moduleName).CreateRows(result.Result as IEnumerable<string>);
                        resultSummary.CreateRow(moduleName, "See details in tab", result.ResultComment);
                        break;
                    case ModuleResultsType.Table:
                        document.CreateSheet(moduleName).CreateRows(result.Result as DataTable);
                        resultSummary.CreateRow(moduleName, "See details in tab", result.ResultComment);
                        break;
                    case ModuleResultsType.ListOfTables:
                        DataSet data = result.Result as DataSet;
                        if (data == null)
                        {
                            resultSummary.CreateRow(moduleName, "Internal error: Invalid DataSet", result.ResultComment);
                            break;
                        }

                        ISheet currentSheet = document.CreateSheet(moduleName);
                        foreach (DataTable tab in data.Tables)
                        {
                            currentSheet.CreateRow(tab);
                        }

                        resultSummary.CreateRow(moduleName, "See details in tab", result.ResultComment);
                        break;
                    default:
                        resultSummary.CreateRow(moduleName, "Internal error: Unknown module", result.ResultComment);
                        continue;
                }
            }

            MemoryStream stream = new MemoryStream();
            document.Write(stream);

            // IWorkbook.Write closes the stream. This is the only way to "re-open" it.
            return new MemoryStream(stream.ToArray());
        }
    }
}
