using Autofac;
using KenticoInspector.Core;
using KenticoInspector.Core.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace KenticoInspector.Reports
{
    public class ReportsModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assemblies = Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => t.IsClass
                    && !t.IsAbstract
                    && typeof(IReport).IsAssignableFrom(t)
                    )
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}
