using Autofac;
using Autofac.Extensions.DependencyInjection;

using KenticoInspector.Core;
using KenticoInspector.Infrastructure;
using KenticoInspector.Reports;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;

namespace KenticoInspector.WebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            return ConfigureAutofac(services);
        }

        private IServiceProvider ConfigureAutofac(IServiceCollection services)
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.Populate(services);

            containerBuilder.RegisterModule(new CoreModule());
            containerBuilder.RegisterModule(new InfrastructureModule());
            containerBuilder.RegisterModule(new ReportsModule());
            containerBuilder.RegisterModule(new ActionsModule());

            var container = containerBuilder.Build();
            return new AutofacServiceProvider(container);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseMvc();
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp/dist";

                if (env.IsDevelopment() && !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ASPNETCORE_USE_EXTERNAL_CLIENT")))
                {
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:8080");
                }
            });
        }
    }
}