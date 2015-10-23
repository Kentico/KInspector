using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class SiteMapModule : IModule
    {
        private Dictionary<string, Type> ColumnsWithInheritance = new Dictionary<string, Type>
        {
                {"IsSecured", typeof(string)},
                {"SSL", typeof(string)},
                {"OutputCache", typeof(int)},
                {"CacheInFile", typeof(string)},
            };
        private List<string> VisibleColumns = new List<string> { "Document", "ChildNodesCount", "Class", "Culture", "Aliases", "Wildcards", "WorkflowScopes", "Attachments", "ACLS", "SKU" };


        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            { 
                Name = "Site map",
                SupportedVersions = new[] { 
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2")
                },
                Comment = @"Site map of the whole content tree",
            };
        }


        public SiteMapModule()
        {
            VisibleColumns.AddRange(ColumnsWithInheritance.Select(x => x.Key));
            VisibleColumns.AddRange(ColumnsWithInheritance.Select(x => x.Key + "Orig"));
        }


        public ModuleResults GetResults(InstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            var sitemaps = dbService.ExecuteAndGetDataSetFromFile("SiteMapModule.sql");

            // Postprocess sitemaps of all sites
            foreach (DataTable sitemap in sitemaps.Tables)
            {
                bool outputCacheEnabled = dbService.GetSetting<bool>("CMSEnableOutputCache", sitemap.TableName);

                // process every row of the sitemap
                foreach (DataRow row in sitemap.Rows)
                {
                    // Get effective value of columns that can be inherited
                    foreach (var column in ColumnsWithInheritance)
                    {
                        string origColName = column.Key + "Orig";

                        // Add new column to the table for storing original column value
                        if (!sitemap.Columns.Contains(origColName))
                        {
                            int colIndex = sitemap.Columns.IndexOf(column.Key);
                            sitemap.Columns.Add(origColName).SetOrdinal(colIndex);
                        }

                        // Copy original value to the new column
                        row[origColName] = row[column.Key];

                        if (column.Key == "OutputCache" && !outputCacheEnabled)
                        {
                            // Special case - output cache can be disabled in settings and then effective value is always 0
                            row[column.Key] = 0;
                        }
                        else
                        {
                            // Set effective value to the column
                            row[column.Key] = GetEffectiveColumnResult(sitemap, row, column.Key, column.Value);
                        }
                    }
                }
                
                // All post processing for the table is done - remove columns that might not be visible
                sitemap.Columns.Cast<DataColumn>()
                    .Select(x => x.ColumnName)
                    .Except(VisibleColumns)
                    .ToList()
                    .ForEach(x => sitemap.Columns.Remove(x));
            }

            return new ModuleResults
            {
                Result = sitemaps,
            };
        }


        private object GetEffectiveColumnResult(DataTable table, DataRow row, string columnName, Type columnType)
        {
            do
            {
                var rowValue = row[columnName];
                if (rowValue.GetType() == columnType)
                {
                    // Effective value is stored in row directly
                    return rowValue;
                }
                else
                {
                    // We have to search in parent node
                    row = table.Select("NodeID = " + row["NodeParentID"]).FirstOrDefault();
                }
            } while (row != null);

            return DBNull.Value;
        }
    }
}
