using System;
using System.Diagnostics;
using Microsoft.Owin.Hosting;
using Kentico.KInspector.WebApplication;

namespace Kentico.KInspector.Server
{
    class Program
    {
        const string BASE_ADDRESS = "http://localhost:9000/";

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

        private static void StartFrontendInBrowser()
        {
            Process.Start(BASE_ADDRESS);
        }

        private static IDisposable StartWebAPI()
        {
            return WebApp.Start<Startup>(BASE_ADDRESS);
        }
    }
}
