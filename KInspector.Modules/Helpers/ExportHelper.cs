using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kentico.KInspector.Core;

using Novacode;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Kentico.KInspector.Modules
{
    public class ExportHelper
    {
        #region Enums and properties

        public enum ExportType
        {
            Xml = 0,
            Xlsx = 1,
            Docx
        }

        private static readonly Dictionary<ExportType, string> MimeTypes = new Dictionary<ExportType, string>
        {
            {ExportType.Xml, "text/xml"},
            {ExportType.Xlsx, "application/xlsx"},
            {ExportType.Docx, "application/docx"}
        };

        private static readonly Dictionary<ExportType, string> Extensions = new Dictionary<ExportType, string>
        {
            {ExportType.Xml, "xml"},
            {ExportType.Xlsx, "xlsx"},
            {ExportType.Docx, "docx"}
        };


        #endregion

        #region Public methods

        public static Stream GetExportStream(IEnumerable<string> moduleNames, InstanceInfo instanceInfo, ExportType type)
        {
            switch (type)
            {
                case ExportType.Xml:
                    return GetExportStreamXml(moduleNames, instanceInfo);
                case ExportType.Xlsx:
                    return GetExportStreamXlsx(moduleNames, instanceInfo);
                case ExportType.Docx:
                    return GetExportStreamDocx(moduleNames, instanceInfo);
                default:
                    throw new ArgumentException(nameof(type));
            }
        }

        public static string GetMimeType(ExportType type)
        {
            return MimeTypes[type];
        }

        public static string GetExtension(ExportType type)
        {
            return Extensions[type];
        }

        #endregion

        #region Private methods

        private static Stream GetExportStreamXml(IEnumerable<string> moduleNames, InstanceInfo instanceInfo)
        {
            throw new NotImplementedException();
        }

        private static Stream GetExportStreamXlsx(IEnumerable<string> moduleNames, InstanceInfo instanceInfo)
        {
            // Create xlsx
            IWorkbook document = new XSSFWorkbook();

            // Create sheet to store results of text modules
            ISheet textModulesSheet = document.CreateSheet("Result summary");
            textModulesSheet.CreateRow("Module", "Result", "Comment");

            ISheet currentSheet = null;

            foreach (string moduleName in moduleNames)
            {
                var result = ModuleLoader.GetModule(moduleName).GetResults(instanceInfo);

                switch (result.ResultType)
                {
                    case ModuleResultsType.String:
                        textModulesSheet.CreateRow(moduleName, result.Result as string, result.ResultComment);
                        break;
                    case ModuleResultsType.List:
                        document.CreateSheet(moduleName).CreateRows(result.Result as IEnumerable<string>);
                        textModulesSheet.CreateRow(moduleName, "Details in tab", result.ResultComment);
                        break;
                    case ModuleResultsType.Table:
                        document.CreateSheet(moduleName).CreateRows(result.Result as DataTable);
                        textModulesSheet.CreateRow(moduleName, "Details in tab", result.ResultComment);
                        break;
                    case ModuleResultsType.ListOfTables:
                        DataSet data = result.Result as DataSet;
                        if (data == null)
                        {
                            textModulesSheet.CreateRow(moduleName, "Invalid DataSet", result.ResultComment);
                            break;
                        }

                        currentSheet = document.CreateSheet(moduleName);
                        foreach (DataTable tab in data.Tables)
                        {
                            currentSheet.CreateRow(tab);
                        }

                        textModulesSheet.CreateRow(moduleName, "Details in tab", result.ResultComment);
                        break;
                    default:
                        textModulesSheet.CreateRow(moduleName, "Internal error: Unknown module", result.ResultComment);
                        continue;
                }
            }

            MemoryStream stream = new MemoryStream();
            document.Write(stream);

            // IWorkbook.Write closes the stream. This is the only way to "re-open" it.
            return new MemoryStream(stream.ToArray());
        }

        private static Stream GetExportStreamDocx(IEnumerable<string> moduleNames, InstanceInfo instanceInfo)
        {
            // Create DocX
            DocX doc = DocX.Load(@"Templates\KInspectorReportTemplate.docx");
            if (doc == null)
            {
                throw new Exception("Could open find export template \"Templates\\KInspectorReportTemplate.docx\"");
            }

            // Process "macros"
            Dictionary<string, string> macros = new Dictionary<string, string>
                {
                    {"SiteName", Convert.ToString(instanceInfo.Uri)},
                    {"SiteVersion", Convert.ToString(instanceInfo.Version)},
                    {"SiteDirectory", Convert.ToString(instanceInfo.Directory)}
                };

            foreach (var macro in macros)
            {
                doc.ReplaceText($"{{% {macro.Key} %}}", macro.Value);
            }

            foreach (string moduleName in moduleNames)
            {
                var result = ModuleLoader.GetModule(moduleName).GetResults(instanceInfo);
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
                        doc.InsertParagraphs($"{moduleName}: ({result.ResultComment})");
                        continue;
                }
            }

            // Save as stream
            MemoryStream memoryStream = new MemoryStream();
            doc.SaveAs(memoryStream);
            return memoryStream;
        }

        #endregion
    }
}
