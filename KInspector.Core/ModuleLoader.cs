using System;
using System.Collections.Generic;
using Ninject;
using Ninject.Extensions.Conventions;

namespace Kentico.KInspector.Core
{
    /// <summary>
    /// Loads all the modules from the assemblies that are in the same directory as an executing assembly.
    /// Thanks to this loader, you can add your own DLL with <see cref="IModule"/> implementations.
    /// </summary>
    public static class ModuleLoader
    {
        private static readonly IDictionary<string, IModule> mModules = new Dictionary<string, IModule>(StringComparer.InvariantCultureIgnoreCase);


        /// <summary>
        /// Loads all the modules from the assemblies that are in the same directory as an executing assembly.
        /// Thanks to this loader, you can add your own DLL with <see cref="IModule"/> implementations.
        /// </summary>
        public static ICollection<IModule> Modules
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
            kernel.Bind(c =>
                    c.FromAssembliesInPath("./")
                        .SelectAllClasses()
                        .InheritedFrom<IModule>()
                        .BindAllInterfaces());

            foreach (var module in kernel.GetAll<IModule>())
            {
                string name = module.GetModuleMetadata().Name;
                if (mModules.ContainsKey(name))
                {
                    throw new ArgumentException("Module with the name '{0}' already exists!", name);
                }

                mModules.Add(name, module);
            }
        }


        public static IModule GetModule(string moduleName)
        {
            if (mModules.Count == 0)
            {
                LoadModules();
            }

            return mModules[moduleName];
        }
    }
}
