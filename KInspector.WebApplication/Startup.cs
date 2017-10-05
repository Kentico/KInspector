using System;
using System.Web.Http;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using Microsoft.Owin.StaticFiles;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles.ContentTypes;

[assembly: OwinStartup(typeof(Kentico.KInspector.WebApplication.Startup))]

namespace Kentico.KInspector.WebApplication
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            // Cors is needed to request localhost server from filesystem
            appBuilder.UseCors(CorsOptions.AllowAll);
            ConfigureWebApi(appBuilder);
            ConfigureStaticFileServing(appBuilder);
        }

        private static void ConfigureStaticFileServing(IAppBuilder appBuilder)
        {
            var physicalFileSystem = new PhysicalFileSystem(@".\WebRoot");
            var options = new FileServerOptions
            {
                EnableDefaultFiles = true,
                FileSystem = physicalFileSystem
            };

            appBuilder.UseFileServer(options);
        }

        private static void ConfigureWebApi(IAppBuilder appBuilder)
        {
            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            appBuilder.UseWebApi(config);
        }
    }
}
