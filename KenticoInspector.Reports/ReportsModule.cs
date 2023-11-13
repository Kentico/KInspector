using Autofac;

using KenticoInspector.Core.Modules;

using System.Reflection;

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