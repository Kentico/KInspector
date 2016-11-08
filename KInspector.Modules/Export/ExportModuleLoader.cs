using System;
using System.Collections.Generic;
using Kentico.KInspector.Core;

using Ninject;
using Ninject.Extensions.Conventions;

namespace Kentico.KInspector.Modules.Export
{
    /// <summary>
    /// Class ensuring automatic load of export modules.
    /// </summary>
    public class ExportModuleLoader
    {
        private static readonly IDictionary<string, IExportModule> mModules = new Dictionary<string, IExportModule>(StringComparer.InvariantCultureIgnoreCase);
        

        /// <summary>
        /// Loads all the modules from the assemblies that are in the same directory as an executing assembly.
        /// Thanks to this loader, you can add your own DLL with <see cref="IModule"/> implementations.
        /// </summary>
        public static ICollection<IExportModule> Modules
        {
            get
            {
                if (mModules.Count == 0)
                {
                    LoadModules();
                }

                return mModules.Values;
            }
        }


        private static void LoadModules()
        {
            var kernel = new StandardKernel();
            kernel.Bind(c => c
                    .FromAssembliesInPath("./")
                        .SelectAllClasses()
                        .InheritedFrom<IExportModule>()
                        .BindAllInterfaces());

            foreach (var module in kernel.GetAll<IExportModule>())
            {
                string name = module.ModuleMetaData.ModuleCodeName;
                if (mModules.ContainsKey(name))
                {
                    throw new ArgumentException("Export module with code name '{0}' already exists!", name);
                }

                mModules.Add(name, module);
            }
        }

        /// <summary>
        /// Return export module of given code name.
        /// </summary>
        /// <param name="moduleCodeName">Code name of the module, as defined in <see cref="ExportModuleMetaData.ModuleCodeName"/></param>
        /// <returns></returns>
        public static IExportModule GetModule(string moduleCodeName)
        {
            if (mModules.Count == 0)
            {
                LoadModules();
            }

            return mModules[moduleCodeName];
        }
    }
}

