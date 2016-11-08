using System;
using System.Xml.Linq;

namespace Kentico.KInspector.Modules.Export.Modules
{
	/// <summary>
	/// Extensions for xml manipulations. Used primarily in <see cref="ExportXml"/>.
	/// </summary>
	public static class ExportXmlExtensions
	{
		/// <summary>
		/// Create xml element containing module summary.
		/// </summary>
		/// <param name="parent">Result sumary element.</param>
		/// <param name="moduleName">Name of the module.</param>
		/// <param name="moduleResult">Module result in string format.</param>
		/// <param name="resultComment">Result comment.</param>
		/// <param name="moduleComment">Module description.</param>
		/// <returns>Created module result sumary xml element.</returns>
		public static XElement AddModuleSummary(this XElement parent, string moduleName, string moduleResult, string resultComment, string moduleComment)
		{
			if (parent == null)
			{
				throw new ArgumentNullException(nameof(parent));
			}

			parent.Add(new XElement("ModuleResultSumary",
				new XElement("Module", moduleName),
				new XElement("Result", moduleResult),
				new XElement("Comment", resultComment),
				new XElement("Description", moduleComment)
			));

			return parent;
		}

		/// <summary>
		/// Adds an XML element with results of an executed module to a parent XML element.
		/// </summary>
		/// <param name="parent">Results element.</param>
		/// <param name="moduleName">Name of the module.</param>
		/// <param name="moduleResult">Module result element.</param>
		/// <param name="moduleComment">Module comment.</param>
		/// <returns>Created module result xml element.</returns>
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