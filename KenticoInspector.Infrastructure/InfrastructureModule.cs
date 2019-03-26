using Autofac;
using KenticoInspector.Core.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace KenticoInspector.Infrastructure
{
    public class InfrastructureModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assemblies = Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => t.IsClass
                    && !t.IsAbstract
                    && typeof(IRepository).IsAssignableFrom(t)
                    )
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}
