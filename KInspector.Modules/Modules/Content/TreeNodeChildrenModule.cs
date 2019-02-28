using System;
using System.Collections.Generic;
using System.Data;

using Kentico.KInspector.Core;
using Kentico.KInspector.Modules.Helpers.StrongTypedInfos;

namespace Kentico.KInspector.Modules
{
    public class TreeNodeChildrenModule : IModule
    {
        private const int TOO_MANY_CHILDREN_THRESHOLD = 1000;

        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Number of children of TreeNode",
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
                Comment = $@"Displays content tree nodes with their number of children.

Our best practice is to store a maximum of {TOO_MANY_CHILDREN_THRESHOLD} children under each tree node, otherwise it can negatively affect the performance.",
                Category = "Content"
            };
        }

        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            var results = dbService.ExecuteAndGetDataSetFromFile("TreeNodeChildrenModule.sql");

            var resultsHaveAProblem = false;
            var status = Status.Good;
            var comment = $"There are no content tree nodes with more than {TOO_MANY_CHILDREN_THRESHOLD} children, everything is OK.";

            var finalDataSet = BuildResultsBySite(results.Tables[0], results.Tables[1], resultsHaveAProblem);

            if (resultsHaveAProblem)
            {
                comment = $"Structure the content in the content tree so that there is no node with {TOO_MANY_CHILDREN_THRESHOLD} or more children.";
                status = Status.Warning;
            }

            return new ModuleResults
            {
                Result = finalDataSet,
                Status = status,
                ResultComment = comment,
            };
        }

        private DataSet BuildResultsBySite(DataTable tableWithFirstColumnBeingSiteID, DataTable siteIDTable, bool resultsHaveAProblem)
        {
            var dataSet = new DataSet();

            for (int i = 0; i < siteIDTable.Rows.Count; i++)
            {
                var siteInfo = new SiteInfo(siteIDTable.Rows[i]);
                var table = GetNodesDataTable(siteInfo.SiteDisplayName);

                foreach (DataRow row in tableWithFirstColumnBeingSiteID.Select($"{nameof(NodeInfo.NodeSiteID)} = {siteInfo.SiteID}"))
                {
                    if (int.Parse(row[nameof(NodeInfo.NodeNumberOfChildren)].ToString()) > TOO_MANY_CHILDREN_THRESHOLD)
                    {
                        resultsHaveAProblem = true;
                    }

                    table.Rows.Add(
                        row[nameof(NodeInfo.NodeAliasPath)],
                        row[nameof(NodeInfo.NodeNumberOfChildren)]
                    );
                }

                dataSet.Tables.Add(table);
            }

            return dataSet;
        }

        private static DataTable GetNodesDataTable(string tableName)
        {
            var nodesTable = new DataTable(tableName);
            nodesTable.Columns.Add(NodeInfo.DisplayNames[nameof(NodeInfo.NodeAliasPath)]);
            nodesTable.Columns.Add(NodeInfo.DisplayNames[nameof(NodeInfo.NodeNumberOfChildren)]);

            return nodesTable;
        }

        private class SiteInfo : SimpleBaseInfo
        {
            public int SiteID => Get<int>(nameof(SiteID));

            public string SiteDisplayName => Get<string>(nameof(SiteDisplayName));

            public SiteInfo(DataRow row) : base(row)
            {
            }
        }

        private class NodeInfo : SimpleBaseInfo
        {
            public int NodeSiteID => Get<int>(nameof(NodeSiteID));

            public string NodeAliasPath => Get<string>(nameof(NodeAliasPath));

            public string NodeNumberOfChildren => Get<string>(nameof(NodeNumberOfChildren));

            public static IDictionary<string, string> DisplayNames = new Dictionary<string, string>{
                { nameof(NodeAliasPath), "Node alias path"},
                { nameof(NodeNumberOfChildren), "Number of children"}
            };

            public NodeInfo(DataRow row) : base(row)
            {
            }
        }
    }
}