using Autofac;

using KInspector.Core.Modules;

using System.Reflection;

namespace KInspector.Reports
{
    public class ActionsModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assemblies = Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => t.IsClass
                    && !t.IsAbstract
                    && typeof(IAction).IsAssignableFrom(t)
                    )
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}