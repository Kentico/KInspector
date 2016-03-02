using System;
using System.Data;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class NumberOfAliasesModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Number of document aliases",
                SupportedVersions = new[] { 
                    new Version("7.0"),
                    new Version("8.0"), 
                    new Version("8.1"),
                    new Version("8.2"),
                    new Version("9.0") },
                Comment = @"Returns number of aliases per node + total count of aliases and documents for direct comparison.

Having too many aliases per node may suggest problem with wrong API usage, decreased performance and SEO problems. 

Huge amount of aliases also decrease site's performance. Only necessary aliases should be kept, the rest should be deleted.",
            };
        }

        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {
            return new ModuleResults
            {
                Result = GetAndJoinDataTables(instanceInfo),
            };
        }

        private DataSet GetAndJoinDataTables(IInstanceInfo instanceInfo)
        {
            var result = instanceInfo.DBService.ExecuteAndGetDataSetFromFile("NumberOfAliasesModule.sql");

            result.Tables[0].TableName = "Number of documents";
            result.Tables[1].TableName = "Number of aliases";
            result.Tables[2].TableName = "Aliases per node";

            return result;
        }
    }
}
