using System;
using System.Collections.Generic;
using System.Data;

using Kentico.KInspector.Core;
using Kentico.KInspector.Modules.Helpers.StrongTypedInfos;

namespace Kentico.KInspector.Modules
{
    public class AttachmentsBySizeModule : IModule
    {
        private const string SITE_DISPLAY_NAME = "SiteDisplayName";

        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Attachments by size",
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
                Comment =
@"Displays report of all attachments ordered by their size.

Storing files in a content tree can negatively affect website performance. If you're not using workflow or multilingual content features to manipulate those attachments, you can move them to the media library.

For more information, see the documentation:
https://docs.kentico.com/k12/developing-websites/defining-website-content-structure?devModel=pe#Definingwebsitecontentstructure-WhenshouldIusepagestostorecontent?",
                Category = "Content"
            };
        }

        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {
            var results = instanceInfo.DBService.ExecuteAndGetDataSetFromFile("AttachmentsBySizeModule.sql");

            var finalDataSet = BuildResultsBySite(results.Tables[0], results.Tables[1]);

            return new ModuleResults
            {
                Result = finalDataSet,
            };
        }

        private DataSet BuildResultsBySite(DataTable tableWithFirstColumnBeingSiteID, DataTable siteIDTable)
        {
            var dataSet = new DataSet();

            for (int i = 0; i < siteIDTable.Rows.Count; i++)
            {
                var siteInfo = new SiteInfo(siteIDTable.Rows[i]);
                var table = GetAttachmentsDataTable(siteInfo.SiteDisplayName);

                foreach (DataRow row in tableWithFirstColumnBeingSiteID.Select($"{nameof(AttachmentInfo.AttachmentSiteID)} = {siteInfo.SiteID}"))
                {
                    table.Rows.Add(
                        row[nameof(AttachmentInfo.NodeAliasPath)],
                        row[nameof(AttachmentInfo.AttachmentName)],
                        row[nameof(AttachmentInfo.AttachmentSize)]
                    );
                }

                dataSet.Tables.Add(table);
            }

            return dataSet;
        }

        private static DataTable GetAttachmentsDataTable(string tableName)
        {
            var attachmentsTable = new DataTable(tableName);
            attachmentsTable.Columns.Add(AttachmentInfo.DisplayNames[nameof(AttachmentInfo.NodeAliasPath)]);
            attachmentsTable.Columns.Add(AttachmentInfo.DisplayNames[nameof(AttachmentInfo.AttachmentName)]);
            attachmentsTable.Columns.Add(AttachmentInfo.DisplayNames[nameof(AttachmentInfo.AttachmentSize)]);

            return attachmentsTable;
        }

        private class SiteInfo : SimpleBaseInfo
        {
            public int SiteID => Get<int>(nameof(SiteID));

            public string SiteDisplayName => Get<string>(nameof(SiteDisplayName));

            public SiteInfo(DataRow row) : base(row)
            {
            }
        }

        private class AttachmentInfo : SimpleBaseInfo
        {
            public int AttachmentSiteID => Get<int>(nameof(AttachmentSiteID));

            public string NodeAliasPath => Get<string>(nameof(NodeAliasPath));

            public string AttachmentName => Get<string>(nameof(AttachmentName));

            public int AttachmentSize => Get<int>(nameof(AttachmentSize));

            public static IDictionary<string, string> DisplayNames = new Dictionary<string, string>{
                { nameof(NodeAliasPath), "Node alias path"},
                { nameof(AttachmentName), "Attachment name"},
                { nameof(AttachmentSize), "Attachment size (bytes)"}
            };

            public AttachmentInfo(DataRow row) : base(row)
            {
            }
        }
    }
}