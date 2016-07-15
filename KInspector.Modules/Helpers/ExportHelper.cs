using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

using System.Xml.Linq;

using Kentico.KInspector.Core;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.XWPF.UserModel;

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

        public static Stream GetExportStream(IEnumerable<string> moduleNames, IInstanceInfo instanceInfo, ExportType type)
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

        private static Stream GetExportStreamXml(IEnumerable<string> moduleNames, IInstanceInfo instanceInfo)
        {
            if (instanceInfo == null)
            {
                throw new ArgumentNullException(nameof(instanceInfo));
            }

            // Create xml
            XElement rootElement = new XElement("KInspectorExport");
            XDocument document = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), rootElement);

            // Create element to store results of text modules
            XElement resultSummary = new XElement("ResultSummary");
            rootElement.Add(resultSummary);

            // Create element to store results of other than text modules
            XElement moduleResults = new XElement("ModuleResults");
            rootElement.Add(moduleResults);

            foreach (string moduleName in moduleNames)
            {
                var result = ModuleLoader.GetModule(moduleName).GetResults(instanceInfo);

                switch (result.ResultType)
                {
                    case ModuleResultsType.String:
                        resultSummary.AddModuleSummary(moduleName, result.Result as string, result.ResultComment);
                        break;

                    case ModuleResultsType.List:
                        if (!(result.Result is IEnumerable<string>))
                        {
                            resultSummary.AddModuleSummary(moduleName, "Internal error: Invalid List", result.ResultComment);
                            break;
                        }

                        XElement listXml = new XElement("Result",
                            ((IEnumerable<string>)result.Result)
                            .Select(resultEntry => new XElement("ResultEntry", resultEntry))
                            .ToArray()
                        );

                        moduleResults.AddModuleResult(moduleName, listXml, result.ResultComment);
                        resultSummary.AddModuleSummary(moduleName, "See module element", result.ResultComment);
                        break;

                    case ModuleResultsType.Table:
                        if (!(result.Result is DataTable))
                        {
                            resultSummary.AddModuleSummary(moduleName, "Internal error: Invalid DataTable", result.ResultComment);
                            break;
                        }

                        using (MemoryStream xmlStream = new MemoryStream())
                        {
                            DataTable table = (DataTable)result.Result;
                            table.TableName = moduleName;
                            table.WriteXml(xmlStream);
                            xmlStream.Seek(0, SeekOrigin.Begin);
                            var resultElement = XElement.Parse(new StreamReader(xmlStream).ReadToEnd());
                            resultElement.Name = "Result";

                            moduleResults.AddModuleResult(moduleName, resultElement, result.ResultComment);
                        }

                        resultSummary.AddModuleSummary(moduleName, "See module element", result.ResultComment);
                        break;

                    case ModuleResultsType.ListOfTables:
                        if (!(result.Result is DataSet))
                        {
                            resultSummary.AddModuleSummary(moduleName, "Internal error: Invalid DataSet", result.ResultComment);
                            break;
                        }

                        var ds = (DataSet) result.Result;
                        ds.DataSetName = "Result";

                        moduleResults.AddModuleResult(moduleName, XElement.Parse(ds.GetXml()), result.ResultComment);
                        resultSummary.AddModuleSummary(moduleName, "See module element", result.ResultComment);
                        break;

                    default:
                        resultSummary.AddModuleSummary(moduleName, "Internal error: Unknown module", result.ResultComment);
                        break;
                }
            }

            MemoryStream stream = new MemoryStream();
            document.Save(stream);

            // IWorkbook.Write closes the stream. This is the only way to "re-open" it.
            return new MemoryStream(stream.ToArray());
        }

        private static Stream GetExportStreamXlsx(IEnumerable<string> moduleNames, IInstanceInfo instanceInfo)
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

        private static Stream GetExportStreamDocx(IEnumerable<string> moduleNames, IInstanceInfo instanceInfo)
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

            // Process "macros"
            Dictionary<string, string> macros = new Dictionary<string, string>
                {
                    {"SiteName", Convert.ToString(instanceInfo.Uri)},
                    {"SiteVersion", Convert.ToString(instanceInfo.Version)},
                    {"SiteDirectory", Convert.ToString(instanceInfo.Directory)}
                };

            foreach (var macro in macros)
            {
                document.ReplaceText($"{{% {macro.Key} %}}", macro.Value);
            }

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

            MemoryStream stream = new MemoryStream();
            document.Write(stream);

            // XWPFDocument.Write closes the stream. This is the only way to "re-open" it.
            return new MemoryStream(stream.ToArray());
        }

        #endregion
    }
}
