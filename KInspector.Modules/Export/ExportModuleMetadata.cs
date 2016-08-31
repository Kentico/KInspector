using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentico.KInspector.Modules.Export
{
    public class ExportModuleMetaData
    {
        public ExportModuleMetaData(string moduleDisplayName, string moduleCodeName, string moduleFileExtension, string moduleFileMimeType)
        {
            ModuleDisplayName = moduleDisplayName;
            ModuleCodeName = moduleCodeName;
            ModuleFileExtension = moduleFileExtension;
            ModuleFileMimeType = moduleFileMimeType;
        }

        public string ModuleDisplayName { get; }

        public string ModuleCodeName { get; }

        public string ModuleFileExtension { get; }

        public string ModuleFileMimeType { get; }
    }
}
