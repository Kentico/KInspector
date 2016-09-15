using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Kentico.KInspector.Core;

using System.Xml.Linq;

namespace Kentico.KInspector.Modules.Export.Modules
{
    public class ExportXml : IExportModule
    {
        /// <summary>
        /// Metadata of the module.
        /// </summary>
        public ExportModuleMetaData ModuleMetaData => new ExportModuleMetaData("Xml", "ExportXml", "xml","text/xml");

        public Stream GetExportStream(IEnumerable<string> moduleNames, IInstanceInfo instanceInfo)
        {
            if (instanceInfo == null)
            {
                throw new ArgumentNullException(nameof(instanceInfo));
            }

            // Create xml root
            XElement rootElement = new XElement("KInspectorExport");
            XDocument document = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), rootElement);


            // Create element to store results of text modules, and sumary of all other modules.
            XElement resultSummary = new XElement("ResultSummary");
            rootElement.Add(resultSummary);

            // Create element to store results of other than text modules
            XElement moduleResults = new XElement("ModuleResults");
            rootElement.Add(moduleResults);

            // Run every module and write its result.
            foreach (string moduleName in moduleNames.Distinct())
            {
                var module = ModuleLoader.GetModule(moduleName);
                var result = module.GetResults(instanceInfo);
                var meta = module.GetModuleMetadata();

                switch (result.ResultType)
                {
                    case ModuleResultsType.String:
                        resultSummary.AddModuleSummary(moduleName, result.Result as string, result.ResultComment, meta.Comment);
                        break;

                    case ModuleResultsType.List:
                        if (!(result.Result is IEnumerable<string>))
                        {
                            resultSummary.AddModuleSummary(moduleName, "Internal error: Invalid List", result.ResultComment, meta.Comment);
                            break;
                        }

                        XElement listXml = new XElement("Result",
                            ((IEnumerable<string>)result.Result)
                            .Select(resultEntry => new XElement("ResultEntry", resultEntry))
                            .ToArray()
                        );

                        moduleResults.AddModuleResult(moduleName, listXml, result.ResultComment);
                        resultSummary.AddModuleSummary(moduleName, "See module element", result.ResultComment, meta.Comment);
                        break;

                    case ModuleResultsType.Table:
                        if (!(result.Result is DataTable))
                        {
                            resultSummary.AddModuleSummary(moduleName, "Internal error: Invalid DataTable", result.ResultComment, meta.Comment);
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

                        resultSummary.AddModuleSummary(moduleName, "See module element", result.ResultComment, module.GetModuleMetadata().Comment);
                        break;

                    case ModuleResultsType.ListOfTables:
                        if (!(result.Result is DataSet))
                        {
                            resultSummary.AddModuleSummary(moduleName, "Internal error: Invalid DataSet", result.ResultComment, module.GetModuleMetadata().Comment);
                            break;
                        }

                        var ds = (DataSet)result.Result;
                        ds.DataSetName = "Result";

                        moduleResults.AddModuleResult(moduleName, XElement.Parse(ds.GetXml()), result.ResultComment);
                        resultSummary.AddModuleSummary(moduleName, "See module element", result.ResultComment, meta.Comment);
                        break;

                    default:
                        resultSummary.AddModuleSummary(moduleName, "Internal error: Unknown module", result.ResultComment, meta.Comment);
                        break;
                }
            }

            MemoryStream stream = new MemoryStream();
            document.Save(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }
    }
}
