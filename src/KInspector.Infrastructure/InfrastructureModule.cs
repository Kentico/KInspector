using System.Reflection;

using Autofac;

using KInspector.Core.Repositories.Interfaces;
using KInspector.Core.Services.Interfaces;

namespace KInspector.Infrastructure
{
    public class InfrastructureModule : Autofac.Module
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