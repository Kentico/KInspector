using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules.Modules.Content
{
    public class NumberOfAliasesModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata()
            {
                Name = "Number of document aliases",
                SupportedVersions = new[] { 
                    new Version("7.0"),
                    new Version("8.0"), 
                    new Version("8.1"),
                    new Version("8.2"),
                    new Version("9.0") },
                Comment = "Returns number of all registered aliases and number of documents. Having too many aliases can decrease site's performance."
            };
        }

        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {
            return new ModuleResults()
            {
                Result = GetAndJoinDataTables(instanceInfo),
            };
        }

        private DataSet GetAndJoinDataTables(IInstanceInfo instanceInfo)
        {
            var ds = new DataSet();

            ds.Tables.Add(GetNumberOfAliases(instanceInfo).Copy());
            ds.Tables.Add(GetNumberOfDocuments(instanceInfo).Copy());
            ds.Tables.Add(GetAliasesPerNode(instanceInfo).Copy());

            return ds;
        }

        private DataTable GetNumberOfDocuments(IInstanceInfo instanceInfo)
        {
            var result = instanceInfo.DBService.ExecuteAndGetTableFromFile("NumberOfAliasesDocumentsModule.sql");

            result.TableName = "1. Number of documents";

            return result;
        }

        private DataTable GetNumberOfAliases(IInstanceInfo instanceInfo)
        {
            var result = instanceInfo.DBService.ExecuteAndGetTableFromFile("NumberOfAliasesModule.sql");

            result.TableName = "2. Number of aliases";

            return result;
        }

        private DataTable GetAliasesPerNode(IInstanceInfo instanceInfo)
        {
            var result = instanceInfo.DBService.ExecuteAndGetTableFromFile("NumberOfAliasesPerNodeModule.sql");

            result.TableName = "3. Aliases per node (ordered by number of aliases)";

            return result;
        }
    }
}
