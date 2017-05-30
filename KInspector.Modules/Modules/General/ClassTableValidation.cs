using System;
using System.Data;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class ClassTableValidation : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            { 
                Name = "Class table validation",
                SupportedVersions = new[] { 
                    new Version("6.0"),
                    new Version("7.0"),
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2"),
                    new Version("9.0"),
                    new Version("10.0")
                },
                Comment = @"Compares Kentico Classes against tables in database, and displays non-matching entries. Lists tables without Class, Classes without specified table, and missing Class tables. Excludes those classes, which are not meant to have a table. ",
                Category = "Database"
            };
        }


        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {
            // Initialize database connection service
            var dbService = instanceInfo.DBService;

            // Retrieve data
            var tablesWithoutClass = dbService.ExecuteAndGetTableFromFile("ClassTableValidationTables.sql");
            tablesWithoutClass.TableName = "Database tables without Kentico Class";
            var classesWithoutTable = dbService.ExecuteAndGetTableFromFile("ClassTableValidationClasses.sql");
            classesWithoutTable.TableName = "Kentico Classes without database table";

            // Merge data into result
            var result = new DataSet("Non-matching Tables-Class entries");
            if (tablesWithoutClass.Rows.Count > 0)
            {
                result.Merge(tablesWithoutClass);
            }
            if (classesWithoutTable.Rows.Count > 0)
            {
                result.Merge(classesWithoutTable);
            }

            // Calculate total number of identified issues (if any)
            int issues = tablesWithoutClass.Rows.Count + classesWithoutTable.Rows.Count;
            
            return new ModuleResults
            {
                Result = result,
                ResultComment = $"{issues} invalid entries found",
                Status = (issues > 0) ? Status.Error : Status.Good
            };
        }
    }
}