using System;
using System.Diagnostics;
using KInspector.Web.WebAPI;
using Microsoft.Owin.Hosting;

namespace KInspector.Web
{
    class Program
    {
        /// <summary>
        /// This console application starts a WebAPI server and opens a web browser with AngularJS application to consume that WebAPI.
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
            Process.Start(".\\FrontEnd\\index.html");
        }

        /// <summary>
        /// Runs the WebAPI that is an interface for analyzing Kentico database.
        /// </summary>
        private static IDisposable StartWebAPI()
        {
            string baseAddress = "http://localhost:9000/";
            return WebApp.Start<Startup>(url: baseAddress);
        }
    }
}
