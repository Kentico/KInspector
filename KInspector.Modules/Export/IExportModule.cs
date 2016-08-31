using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules.Export
{
    public interface IExportModule
    {
        ExportModuleMetaData ModuleMetaData { get; }

        Stream GetExportStream(IEnumerable<string> moduleNames, IInstanceInfo instanceInfo);
    }
}
