using System.Web.Http;
using Microsoft.Owin.Cors;
using Owin;

namespace Kentico.KInspector.Web
{
	/// <summary>
	/// Configures WebAPI (Self-host).
	/// </summary>
	public class Startup
	{
		public void Configuration(IAppBuilder appBuilder)
		{
			// Cors is needed to request localhost server from filesystem
			appBuilder.UseCors(CorsOptions.AllowAll);

			HttpConfiguration config = new HttpConfiguration();
			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{action}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);
			appBuilder.UseStaticFiles("/FrontEnd");
			appBuilder.UseWebApi(config);
		}
	}
}