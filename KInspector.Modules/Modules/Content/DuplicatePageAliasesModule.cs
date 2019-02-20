using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Kentico.KInspector.Core;
using Kentico.KInspector.Modules.Helpers.StrongTypedInfos;

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
                    new Version("9.0"),
                    new Version("10.0"),
                    new Version("11.0")
                },
                Comment = @"Checks that the CMS_DocumentAlias and CMS_Tree tables do not contain any duplicates.",
                Category = "Content"
            };
        }

        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            var results = instanceInfo.DBService.ExecuteAndGetDataSetFromFile("DuplicatePageAliasesModule.sql");

            if (results.Tables[0].Rows.Count > 0)
            {
                var finalDataSet = BuildResultsBySite(results.Tables[0], results.Tables[1]);

                return new ModuleResults
                {
                    Result = finalDataSet,
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

        private DataSet BuildResultsBySite(DataTable tableWithFirstColumnBeingSiteID, DataTable siteIDTable)
        {
            var dataSet = new DataSet();

            for (int i = 0; i < siteIDTable.Rows.Count; i++)
            {
                var siteInfo = new SiteInfo(siteIDTable.Rows[i]);
                var table = GetAttachmentsDataTable(siteInfo.SiteDisplayName);

                foreach (DataRow row in tableWithFirstColumnBeingSiteID.Select($"{nameof(AliasInfo.AliasSiteID)} = {siteInfo.SiteID}"))
                {
                    table.Rows.Add(
                        GetAliasRowWithParsedReason(new AliasInfo(row))
                    );
                }

                dataSet.Tables.Add(table);
            }

            return dataSet;
        }

        private static DataTable GetAttachmentsDataTable(string tableName)
        {
            var attachmentsTable = new DataTable(tableName);
            attachmentsTable.Columns.Add(AliasInfo.DisplayNames[nameof(AliasInfo.AliasURLPath)]);
            attachmentsTable.Columns.Add(AliasInfo.DisplayNames[nameof(AliasInfo.AffectedNodeIDs)]);
            attachmentsTable.Columns.Add(AliasInfo.DisplayNames[nameof(AliasInfo.OriginalNodeID)]);
            attachmentsTable.Columns.Add(AliasInfo.DisplayNames[nameof(AliasInfo.Reasons)]);

            return attachmentsTable;
        }

        private object[] GetAliasRowWithParsedReason(AliasInfo alias)
        {
            var nodeIDs = alias.AffectedNodeIDs
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(i => int.Parse(i.ToString()));

            int originalNodeID = alias.OriginalNodeID;

            var reasons = string.Empty;

            // No duplicate IDs means that the duplicates are all in CMS_DocumentAlias
            if (!AnyDuplicates(nodeIDs))
            {
                reasons += "Same alias for different nodes in CMS_DocumentAlias. ";
            }

            // Original node ID means that one of the duplicates is in CMS_Tree
            if (originalNodeID > 0)
            {
                reasons += "Duplicate alias in CMS_DocumentAlias as the original alias in CMS_Tree. ";
            }

            // Any duplicates exist which are not the original node ID
            if (AnyDuplicates(nodeIDs.Where(i => !i.Equals(originalNodeID))))
            {
                reasons += "Duplicate aliases for the same node in CMS_DocumentAlias.";
            }

            return new object[] {
                alias.AliasURLPath,
                string.Join(", ", nodeIDs),
                alias.OriginalNodeID,
                reasons
            };
        }

        private static bool AnyDuplicates(IEnumerable<int> nodeIDs)
        {
            return nodeIDs
                .GroupBy(x => x)
                .Where(g => g.Count() > 1)
                .Any();
        }

        private class SiteInfo : SimpleBaseInfo
        {
            public int SiteID => Get<int>(nameof(SiteID));

            public string SiteDisplayName => Get<string>(nameof(SiteDisplayName));

            public SiteInfo(DataRow row) : base(row)
            {
            }
        }

        private class AliasInfo : SimpleBaseInfo
        {
            public int AliasSiteID => Get<int>(nameof(AliasSiteID));

            public string AliasURLPath => Get<string>(nameof(AliasURLPath));

            public string AffectedNodeIDs => Get<string>(nameof(AffectedNodeIDs));

            public int OriginalNodeID => Get<int>(nameof(OriginalNodeID));

            public string Reasons => "Reasons";

            public static IDictionary<string, string> DisplayNames = new Dictionary<string, string>{
                { nameof(AliasURLPath), "Alias URL path"},
                { nameof(AffectedNodeIDs), "Affected node IDs"},
                { nameof(OriginalNodeID), "Original node ID"},
                { nameof(Reasons), "Reasons"}
            };

            public AliasInfo(DataRow row) : base(row)
            {
            }
        }
    }
}