using System;
using System.Data;
using System.Linq;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class DocumentsConsistencyIssuesModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            { 
                Name = "Documents consistency issues",
                SupportedVersions = new[] { 
                    new Version("6.0"),
                    new Version("7.0"),
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2"),
                    new Version("9.0")
                },
                Comment = @"Checks that CMS_Tree and CMS_Document tables are without any consistency issues.",
                Category = "Consistency issues"
            };
        }

        public ModuleResults GetResults(InstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            DataSet results = dbService.ExecuteAndGetDataSetFromFile("DocumentsConsistencyIssuesModule.sql");

            RemoveEmptyTables(results);

            if (results.Tables.Count != 0)
            {
                return new ModuleResults
                {
                    Result = results,
                    ResultComment = "Consistency issues found!",
                    Status = Status.Error,
                };
            }

            return new ModuleResults
            {
                Status = Status.Good,
                ResultComment = "No consistency issues found."
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
