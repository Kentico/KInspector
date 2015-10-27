using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class UIElementsDiff : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "UIElements diff",
                SupportedVersions = new[] { 
                    new Version("5.5"),
                    new Version("7.0"),
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2")
                },
                Comment = "Compares UIElements table with Kentico's default instalation and displays all differences."
            };
        }


        public ModuleResults GetResults(InstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            DataTable sourceUIElements = dbService.ExecuteAndGetTableFromFile(String.Format("UIElementsDiffV{0}.sql", instanceInfo.Version.Major));

            var kenticoUIElements = new DataTable();
            kenticoUIElements.ReadXml(String.Format("./Data/DefaultUIElements/{0}{1}.xml", instanceInfo.Version.Major, instanceInfo.Version.Minor));

            RemoveHashesFromStringRecords(sourceUIElements);

            var result = Diff(kenticoUIElements, sourceUIElements);

            return new ModuleResults
            {
                Result = result
            };
        }


        private static DataTable Diff(DataTable source, DataTable target)
        {
            DataTable results = new DataTable();
            results.Columns.Add("ElementName", typeof(string));
            results.Columns.Add("ColumnName", typeof(string));
            results.Columns.Add("Kentico value", typeof(string));
            results.Columns.Add("Source value", typeof(string));

            foreach (DataRow sourceRow in source.Rows)
            {
                foreach (DataColumn col in source.Columns)
                {
                    var targetRow = target.Select("ElementGUID = '" + sourceRow["ElementGUID"] + "'").FirstOrDefault();

                    if (targetRow == null)
                    {
                        results.Rows.Add(sourceRow["ElementName"], sourceRow["ElementGUID"], "Element is missing.");
                    }
                    else
                    {
                        string originalValue = sourceRow[col.ColumnName].ToString().Replace("\n", "").Replace("\r", "").Replace("\t", "");
                        string targetValue = targetRow[col.ColumnName].ToString().Replace("\n", "").Replace("\r", "").Replace("\t", "");
                        if (!String.Equals(originalValue, targetValue, StringComparison.InvariantCultureIgnoreCase))
                        {
                            results.Rows.Add(sourceRow["ElementName"], col.ColumnName, originalValue, targetValue);
                        }
                    }
                }
            }

            return results;
        }


        private static void RemoveHashesFromStringRecords(DataTable table)
        {
            foreach (DataColumn col in table.Columns)
            {
                if (col.DataType != typeof(string))
                {
                    continue;
                }

                foreach (DataRow row in table.Rows)
                {
                    row[col.ColumnName] = RemoveHash(row[col.ColumnName].ToString());
                }
            }

            table.AcceptChanges();
        }


        private static Regex hashMatch = new Regex(@"\|(\(hash\)).{64}", RegexOptions.Compiled);

        public static string RemoveHash(string input)
        {
            return hashMatch.Replace(input, "");
        }
    }
}
