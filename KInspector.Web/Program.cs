using System;
using System.Diagnostics;
using Microsoft.Owin.Hosting;

namespace Kentico.KInspector.Web
{
    class Program
	{
		/// <summary>
		/// Server's base address
		/// </summary>
		const string BASE_ADDRESS = "http://localhost:9000/";

		/// <summary>
		/// This console application starts a WebAPI server and opens a web browser 
		/// with AngularJS application to consume that WebAPI.
		/// </summary>
		static void Main(string[] args)
		{
			using (StartWebAPI())
			{
				StartFrontendInBrowser();

				do
				{
					Console.Clear();
					Console.WriteLine("Server started, press q for shutdown");
				} while (Console.ReadKey().KeyChar != 'q');

				Console.Clear();
				Console.WriteLine("Shutting down the server");
			}
		}

		/// <summary>
		/// Starts Chrome/FF/IE with Angular application consuming the WebAPI.
		/// </summary>
		private static void StartFrontendInBrowser()
		{
			Process.Start(BASE_ADDRESS + "FrontEnd/index.html");
		}

		/// <summary>
		/// Runs the WebAPI that is an interface for analyzing Kentico database.
		/// </summary>
		private static IDisposable StartWebAPI()
		{
			return WebApp.Start<Startup>(BASE_ADDRESS);
		}
	}
}
