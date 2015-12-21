using System;
using System.Data;
using System.Linq;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class DuplicatePageAliasesModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            { 
                Name = "Duplicate Page Aliases",
                SupportedVersions = new[] { 
                    new Version("6.0"),
                    new Version("7.0"),
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2"),
                    new Version("9.0")
                },
                Comment = @"Checks that the CMS_DocumentAlias table does not contain any duplicates.",
                Category = "Content"
            };
        }

        public ModuleResults GetResults(InstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            DataSet results = dbService.ExecuteAndGetDataSetFromFile("DuplicatePageAliasesModule.sql");

            RemoveEmptyTables(results);

            if (results.Tables.Count != 0)
            {
                return new ModuleResults
                {
                    Result = results,
                    ResultComment = "Document Alias issues found!",
                    Status = Status.Error,
                };
            }

            return new ModuleResults
            {
                Status = Status.Good,
                ResultComment = "No issues found."
            };
        }


        private void RemoveEmptyTables(DataSet dataSet)
        {
            var emptyTables = dataSet.Tables.Cast<DataTable>()
                .Where(table => table.Rows.Count == 0).ToList();

            foreach (var emptyTable in emptyTables)
            {
                dataSet.Tables.Remove(emptyTable);
            }
        }
    }
}
