using System;
using System.Net;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class RobotsTxtModule: IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            { 
                Name = "Robots.txt", 
                SupportedVersions = new[] { 
                    new Version("6.0"),
                    new Version("7.0"),
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2")
                },
                Comment = @"Checks that the robots.txt file is present. See http://www.robotstxt.org/robotstxt.html for more details",
                
                Category = "Content",
            };
        }

        public ModuleResults GetResults(InstanceInfo instanceInfo)
        {
            if (!TestUrl(instanceInfo.Uri, "robots.txt"))
            {
                return new ModuleResults
                {
                    Status = Status.Warning,
                    Result = "Missing! Please add the robots.txt into the web root",
                };
            }

            return new ModuleResults
            {
                Status = Status.Good,
                Result = "All good, robots.txt found.",
            };
        }

        private static bool TestUrl(Uri url, string file)
        {
            HttpWebRequest request = WebRequest.CreateHttp(new Uri(url, file));
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                return response.StatusCode == HttpStatusCode.OK;
            }
        }
    }
}
