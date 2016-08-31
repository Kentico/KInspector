using System;
using System.Xml.Linq;

namespace Kentico.KInspector.Modules
{
    public static class ExportXmlExtensions
    {
        public static XElement AddModuleSummary(this XElement parent, string moduleName, string moduleResult, string moduleComment)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            parent.Add(new XElement("ModuleResultSumary",
                new XElement("Module", moduleName),
                new XElement("Result", moduleResult),
                new XElement("Comment", moduleComment)
            ));
            
            return parent;
        }

        public static XElement AddModuleResult(this XElement parent, string moduleName, XElement moduleResult, string moduleComment)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            parent.Add(new XElement("ModuleResult",
                new XElement("Module", moduleName),
                new XElement("Comment", moduleComment),
                moduleResult
            ));

            return parent;
        }

    }
}