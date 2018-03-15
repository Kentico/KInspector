using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
                    new Version("10.0"),
                    new Version("11.0")
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
            var tablesWithoutClassCount = tablesWithoutClass.Select($"TABLE_NAME not in ({formattedWhitelist})").Count();
            
            var classesWithoutTable = dbService.ExecuteAndGetTableFromFile("ClassTableValidationClasses.sql");
            classesWithoutTable.TableName = "Kentico Classes without database table";
            var classesWithoutTableCount = classesWithoutTable.Rows.Count;

            // Merge data into result
            var result = new DataSet("Non-matching Tables-Class entries");
            
            if (tablesWithoutClassCount > 0)
            {
                result.Merge(tablesWithoutClass);
            }
            if (classesWithoutTableCount > 0)
            {
                result.Merge(classesWithoutTable);
            }

            // Calculate total number of identified issues (if any)
            int issues = tablesWithoutClassCount + classesWithoutTableCount;

            return new ModuleResults
            {
                Result = result,
                ResultComment = $"{issues} invalid entries found",
                Status = (issues > 0) ? Status.Error : Status.Good
            };
        }
        
        private List<string> GetTableWhitelist(Version version)
        {
            var whitelist = new List<string>();
            
            if (version.Major >= 10)
            {
               whitelist.Add("CI_Migration");
            }

            return whitelist;
        }
    }
}