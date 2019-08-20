﻿using System.Reflection;

using Autofac;

using KenticoInspector.Core.Repositories.Interfaces;
using KenticoInspector.Core.Services.Interfaces;

namespace KenticoInspector.Core
{
    public class CoreModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assemblies = Assembly.GetExecutingAssembly();

            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => t.IsClass
                    && !t.IsAbstract
                    && typeof(IService).IsAssignableFrom(t)
                    )
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

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